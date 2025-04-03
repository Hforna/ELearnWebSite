using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Domain.Entities;
using Payment.Domain.Repositories;
using Payment.Domain.Services.RabbitMq;
using Payment.Domain.Services.Rest;
using SharedMessages.PaymentMessages;
using Sqids;
using Stripe.Issuing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        public WebhookService(IUnitOfWork uof, ILogger<WebhookService> logger, 
            IUserRestService userRest, ICourseProducerService courseProducer, 
            SqidsEncoder<long> sqids, ICourseRestService courseRest)
        {
            _uof = uof;
            _logger = logger;
            _userRest = userRest;
            _courseProducer = courseProducer;
            _sqids = sqids;
            _courseRest = courseRest;
        }

        public async Task PixTrioWebhook(TrioWebhookPayload payload)
        {
            var userId = _sqids.Decode(payload.Data.ExternalId).SingleOrDefault();

            var transaction = await _uof.transactionRead.TransactionByGatewayId(payload.Data.Id);

            if (payload.Type == "settled")
                await ProcessSettledTransaction(userId, transaction);
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

            var balances = new List<Balance>();
            foreach (var teacher in teacherPrices)
            {
                var balance = await _uof.balanceRead.BalanceByTeacherId(teacher.Key);
                if (balance == null)
                {
                    _logger.LogWarning("Balance record not found for teacher ID: {TeacherId}", teacher.Key);
                    continue;
                }

                balance.BlockedBalance += teacher.Value;
                balance.UpdatedOn = DateTime.UtcNow;
                balances.Add(balance);
            }

            if (balances.Count > 0)
            {
                _uof.balanceWrite.UpdateRange(balances);
            }

            await _uof.Commit();
        }
    }
}
