using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Extensions;
using Payment.Application.Requests;
using Payment.Domain.Cons;
using Payment.Domain.Entities;
using Payment.Domain.Exceptions;
using Payment.Domain.Repositories;
using Payment.Domain.Services.RabbitMq;
using Payment.Domain.Services.Rest;
using SharedMessages.PaymentMessages;
using Sqids;
using Stripe;
using Stripe.Issuing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.ApplicationServices
{
    public class WebhookService : IWebhookService
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger<WebhookService> _logger;
        private readonly IUserRestService _userRest;
        private readonly ICourseProducerService _courseProducer;
        private readonly SqidsEncoder<long> _sqids;
        private readonly ICourseRestService _courseRest;
        private readonly ICurrencyExchangeService _currencyExchange;

        public WebhookService(IUnitOfWork uof, ILogger<WebhookService> logger, 
            IUserRestService userRest, ICourseProducerService courseProducer, 
            SqidsEncoder<long> sqids, ICourseRestService courseRest, ICurrencyExchangeService currencyExchange)
        {
            _uof = uof;
            _currencyExchange = currencyExchange;
            _logger = logger;
            _userRest = userRest;
            _courseProducer = courseProducer;
            _sqids = sqids;
            _courseRest = courseRest;
        }

        public async Task BalanceTransferStripeWebhook(Event payload)
        {
            var content = payload.Data.Object as Transfer;
            var userId = long.Parse(content.Metadata.GetValueOrDefault("user_id"));

            var payout = await _uof.payoutRead.PayoutByUserId(userId);

            if(payout.TransactionStatus == Domain.Enums.TransactionStatusEnum.Pending)
            {
                payout.Active = false;
                payout.ProcessedAt = DateTime.UtcNow;

                if (payload.Type == EventTypes.ChargeSucceeded)
                {
                    payout.TransactionStatus = Domain.Enums.TransactionStatusEnum.Approved;
                }
                else if (payload.Type == EventTypes.ChargeFailed)
                {
                    var balance = await _uof.balanceRead.BalanceByTeacherId(userId);
                    balance.AvaliableBalance += content.Amount;

                    payout.TransactionStatus = Domain.Enums.TransactionStatusEnum.Canceled;
                }
                _uof.payoutWrite.Update(payout);
                await _uof.Commit();
            }
        }

        public async Task CardStripeWebhook(Stripe.Event payload)
        {
            if(payload.Type == EventTypes.PaymentIntentSucceeded)
            {
                var content = payload.Data.Object as PaymentIntent;
                var userId = content.Metadata.GetValueOrDefault("user_id");

                var transaction = await _uof.transactionRead.TransactionByGatewayId(content.Id);

                if (transaction is null)
                    throw new PaymentException("Transaction doesn't exists", System.Net.HttpStatusCode.InternalServerError);

                await ProcessSettledTransaction(_sqids.Decode(userId).Single(), transaction!);
            }
        }

        public async Task PixTrioWebhook(TrioWebhookPayload payload)
        {
            var userId = _sqids.Decode(payload.Data.ExternalId).SingleOrDefault();

            var transaction = await _uof.transactionRead.TransactionByGatewayId(payload.Data.Id);

            if (transaction is null)
                throw new PaymentException("Transaction doesn't exists", System.Net.HttpStatusCode.InternalServerError);

            if (payload.Type == "settled")
                await ProcessSettledTransaction(userId, transaction);
        }

        public async Task RefundOrderStripeWebhook(Event payload)
        {
            if(payload.Type == EventTypes.RefundCreated)
            {
                var obj = payload.Data.Object as Refund;
                var userId = obj.Metadata["user_id"];
                var courseId = long.Parse(obj.Metadata["course_id"]);

                var course = await _courseRest.GetCourse(_sqids.Encode(courseId));

                var order = await _uof.orderRead.LastCourseOrderItem(courseId);

                if (order is null)
                    throw new Exception("Course's order item doesn't exists");

                var teacherId = _sqids.Decode(course.teacherId).Single();
                var teacherBalance = await _uof.balanceRead.BalanceByTeacherId(teacherId);

                var exchange = await _currencyExchange.GetCurrencyRates(DefaultCurrency.Currency);

                switch(course.currencyType)
                {
                    case Domain.Enums.CurrencyEnum.BRL:
                        course.price *= exchange.BRL;
                        break;
                    case Domain.Enums.CurrencyEnum.EUR:
                        course.price *= exchange.EUR;
                        break;
                    case Domain.Enums.CurrencyEnum.USD:
                        course.price *= exchange.USD;
                        break;
                }

                teacherBalance.AvaliableBalance -= course.price;
                
                _uof.balanceWrite.Update(teacherBalance);
                await _uof.Commit();

                await _courseProducer.SendCourseRefunded(new UserGotRefundMessage() { CourseIds = new List<long>() { courseId }, UserId = long.Parse(userId) });
            }
        }

        private async Task ProcessSettledTransaction(long userId, Payment.Domain.Entities.Transaction transaction)
        {
            transaction.TransactionStatus = Domain.Enums.TransactionStatusEnum.Approved;
            transaction.UpdatedOn = DateTime.UtcNow;
            _uof.transactionWrite.Update(transaction);

            var payment = await _uof.paymentRead.PaymentByUser(userId);
            if (payment != null)
            {
                _uof.paymentWrite.Delete(payment);
            }

            var coursesIds = transaction.Order.OrderItems.Select(d => d.CourseId).ToList();
            await _courseProducer.SendAllowCourseToUser(new AllowCourseToUserMessage
            {
                CoursesIds = coursesIds,
                UserId = userId
            });

            var teacherPrices = new ConcurrentDictionary<long, decimal>();
            var courseTasks = transaction.Order.OrderItems.Select(async item =>
            {
                var courseId = _sqids.Encode(item.CourseId);
                var course = await _courseRest.GetCourse(courseId);

                if (course == null)
                {
                    _logger.LogWarning("Course not found for ID: {CourseId}", item.CourseId);
                    return;
                }

                var teacherIds = _sqids.Decode(course.teacherId);
                if (teacherIds.Count == 0)
                {
                    _logger.LogWarning("Invalid teacher ID format: {TeacherId}", course.teacherId);
                    return;
                }

                var teacherId = teacherIds[0];
                var teacherTax = (decimal)Math.Round((double)item.Price * 0.60, 2);
                teacherPrices.AddOrUpdate(teacherId, teacherTax, (_, existing) => existing + teacherTax);
            });

            await Task.WhenAll(courseTasks);

            var balances = new List<Payment.Domain.Entities.Balance>();
            foreach (var teacher in teacherPrices)
            {
                var balance = await _uof.balanceRead.BalanceByTeacherId(teacher.Key);
                if (balance == null)
                {
                    _logger.LogWarning("Balance record not found for teacher ID: {TeacherId}", teacher.Key);
                    continue;
                }

                var blockedBalance = new BlockedBalance() { Amount = teacher.Value, Id = balance.Id };
                await _uof.balanceWrite.AddBlockedBalance(blockedBalance);

                balance.UpdatedOn = DateTime.UtcNow;
                balances.Add(balance);
            }

            if (balances.Count > 0)
            {
                _uof.balanceWrite.UpdateRange(balances);
            }

            transaction.Order.OrderItems.Select(d =>
            {
                d.Active = false;

                return d;
            });
            transaction.Order.Active = false;
            _uof.orderWrite.UpdateOrder(transaction.Order);
            _uof.orderWrite.UpdateOrderItemRange(transaction.Order.OrderItems.ToList());

            await _uof.Commit();
        }
    }
}
