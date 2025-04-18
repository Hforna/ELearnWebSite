using AutoMapper;
using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Application.Responses.Payment;
using Payment.Domain.Cons;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Repositories;
using Payment.Domain.Services.Payment.PaymentInterfaces;
using Payment.Domain.Services.PaymentInterfaces;
using Payment.Domain.Services.RabbitMq;
using Payment.Domain.Services.Rest;
using SharedMessages.PaymentMessages;
using Sqids;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Payment.Application.ApplicationServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IPixGatewayService _pixService;
        private readonly IUserRestService _userService;
        private readonly ILocationRestService _locationRest;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly ICourseProducerService _courseProducer;
        private readonly IPaymentGatewayService _paymentGateway;
        private readonly ICourseRestService _courseRest;
        private readonly ICurrencyExchangeService _currencyExchange;

        public PaymentService(IUnitOfWork uof, SqidsEncoder<long> sqids, 
            IPixGatewayService pixService,
            IUserRestService userService, ILocationRestService locationRest,
            IMapper mapper, ILogger<PaymentService> logger,
            ICourseProducerService courseProducer, IPaymentGatewayService paymentGateway, 
            ICourseRestService courseRest, ICurrencyExchangeService currencyExchange)
        {
            _uof = uof;
            _sqids = sqids;
            _courseRest = courseRest;
            _currencyExchange = currencyExchange;
            _courseProducer = courseProducer;
            _logger = logger;
            _pixService = pixService;
            _userService = userService;
            _locationRest = locationRest;
            _mapper = mapper;
            _paymentGateway = paymentGateway;
        }

        public async Task<PaymentCardResponse> ProcessCardPayment(CardPaymentRequest request)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var userOrder = await _uof.orderRead.OrderByUserId(userId);

            if (userOrder is null || userOrder.OrderItems is null || userOrder.OrderItems.Any() == false)
                throw new OrderException(ResourceExceptMessages.ORDER_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var userCurrency = await _locationRest.GetCurrencyByUserLocation();
            var sumOrder = userOrder.OrderItems.Sum(d => d.Price);

            var currencyPayment = Enum.TryParse(typeof(CurrencyEnum), userCurrency.Code, true, out var result) ? (CurrencyEnum)result : DefaultCurrency.Currency;
            var amountPayment = sumOrder;

            if(currencyPayment != userOrder.Currency)
            {
                var exchange = await _currencyExchange.GetCurrencyRates(userOrder.Currency);

                switch(currencyPayment)
                {
                    case CurrencyEnum.BRL:
                        amountPayment *= (decimal)exchange.BRL;
                        break;
                    case CurrencyEnum.EUR:
                        amountPayment *= (decimal)exchange.EUR;
                        break;
                    case CurrencyEnum.USD:
                        amountPayment *= (decimal)exchange.USD;
                        break;
                    default:
                        break;
                }
            }

            dynamic payment = request.OnCredit ? await _paymentGateway.CreditCardPayment(request.FirstName, request.LastName, 
                request.CardToken, amountPayment, currencyPayment, user.id, request.Installments) : 
                await _paymentGateway.DebitCardPayment(request.FirstName, request.LastName, request.CardToken, amountPayment, currencyPayment, user.id);

            var transaction = new Transaction()
            {
                Amount = amountPayment,
                Currency = currencyPayment,
                OrderId = userOrder.Id,
                TransactionStatus = Enum.TryParse(typeof(TransactionStatusEnum), payment.Status, true, out dynamic status) ? (TransactionStatusEnum)status : TransactionStatusEnum.Processing,
                GatewayTransactionId = payment.Id,
            };
            await _uof.transactionWrite.Add(transaction);

            if (payment.Status == "canceled")
            {
                await _uof.Commit();
                throw new PaymentException(ResourceExceptMessages.PAYMENT_CANCELED, System.Net.HttpStatusCode.InternalServerError);
            }

            var paymentEntity = new PaymentEntity()
            {
                Amount = amountPayment,
                Currency = currencyPayment,
                CustomerId = userId,
                PaymentMethodType = request.OnCredit ? PaymentMethodEnum.Credit : PaymentMethodEnum.Debit,
                TokenizedData = request.CardToken
            };
            await _uof.paymentWrite.Add(paymentEntity);
            await _uof.Commit();

            var response = new PaymentCardResponse()
            {
                Amount = amountPayment,
                Currency = currencyPayment,
                PaymentId = paymentEntity.Id,
                TransactionId = transaction.Id,
                Installments = request.Installments,
                Status = payment.Status,
                Success = payment.Success,
                ClientSecret = payment.ClientSecret,
                RequiresAction = payment.RequiresAction
            };
            return response;
        }

        public async Task<PaymentPixResponse> ProcessPixPayment(PixPaymentRequest request)
        {
            var cpfRegex = Regex.IsMatch(request.Cpf, @"^\d{11}$");

            if (cpfRegex == false)
            {
                _logger.LogError($"Invalid cpf: {MaskCpf(request.Cpf)}");
                throw new PaymentException(ResourceExceptMessages.CPF_FORMAT_NOT_VALID, System.Net.HttpStatusCode.BadRequest);
            }

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var userOrder = await _uof.orderRead.OrderByUserId(userId);

            if (userOrder is null)
                throw new OrderException(ResourceExceptMessages.ORDER_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var userCurrency = await _locationRest.GetCurrencyByUserLocation();
            var userCurrencyAsEnum = Enum.TryParse(typeof(CurrencyEnum), userCurrency.Code, out var result) ? (CurrencyEnum)result : DefaultCurrency.Currency;

            if (userCurrencyAsEnum != CurrencyEnum.BRL)
                throw new PaymentException(ResourceExceptMessages.CURRENCY_NOT_ALLOWED, System.Net.HttpStatusCode.Unauthorized);

            var payment = new PaymentEntity()
            {
                Amount = userOrder.TotalPrice,
                Currency = CurrencyEnum.BRL,
                CustomerId = userId,
                PaymentMethodType = PaymentMethodEnum.Pix,
            };

            var sendPayment = await _pixService.ProcessPixTransaction(request.Cpf, user.email, request.FirstName, request.LastName, payment.Amount);
            TransactionStatusEnum statusTransaction;

            _logger.LogInformation($"Pix payment status: {sendPayment.Status}");

            switch(sendPayment.Status)
            {
                case "approved":
                    statusTransaction = TransactionStatusEnum.Approved;
                    break;
                case "pending":
                    statusTransaction = TransactionStatusEnum.Pending;
                    break;
                case "canceled":
                    statusTransaction = TransactionStatusEnum.Canceled;
                    break;
                default:
                    statusTransaction = TransactionStatusEnum.Pending;
                    break;
            }

            var transaction = new Transaction()
            {
                Amount = userOrder.TotalPrice,
                Currency = userCurrencyAsEnum,
                OrderId = userOrder.Id,
                TransactionStatus = statusTransaction,
                GatewayTransactionId = sendPayment.Id
            };

            await _uof.transactionWrite.Add(transaction);
            await _uof.paymentWrite.Add(payment);
            _uof.orderWrite.UpdateOrder(userOrder);

            await _uof.Commit();

            var response = _mapper.Map<PaymentPixResponse>(sendPayment);
            response.GatewayId = sendPayment.Id;
            response.TransactionId = transaction.Id;
            response.PaymentId = payment.Id;

            return response;
        }

        public async Task<RefundResponse> RequestCourseRefund(long courseId)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _courseRest.GetCourse(_sqids.Encode(courseId));

            var orderItem = await _uof.orderRead.LastCourseOrderItem(courseId);

            if (orderItem is null || orderItem.Active)
                throw new OrderException(ResourceExceptMessages.USER_DOESNT_HAVE_THE_COURSE, System.Net.HttpStatusCode.NotFound);

            var transaction = await _uof.transactionRead.TransactionByOrderId(orderItem.OrderId);

            var refund = await _paymentGateway.RefundUserCourse(courseId, userId, transaction.Amount, transaction.Currency, transaction.GatewayTransactionId);

            return new RefundResponse()
            {
                CourseId = course.id,
                GatewayId = refund.GatewayId,
                Reason = refund.Reason,
                Status = refund.Status
            };
        }

        string MaskCpf(string cpf) => string.Concat(cpf.AsSpan(0, 3), ".***.***-**");
    }
}
