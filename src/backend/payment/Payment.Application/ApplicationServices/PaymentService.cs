using AutoMapper;
using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Application.Responses.Payment;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Repositories;
using Payment.Domain.Services.Payment.PaymentInterfaces;
using Payment.Domain.Services.PaymentInterfaces;
using Payment.Domain.Services.Rest;
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

        public PaymentService(IUnitOfWork uof, SqidsEncoder<long> sqids, 
            IPixGatewayService pixService, 
            IUserRestService userService, ILocationRestService locationRest, IMapper mapper, ILogger<PaymentService> logger)
        {
            _uof = uof;
            _sqids = sqids;
            _logger = logger;
            _pixService = pixService;
            _userService = userService;
            _locationRest = locationRest;
            _mapper = mapper;
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
            var userCurrencyAsEnum = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), userCurrency.Code);

            if (userCurrencyAsEnum != CurrencyEnum.BRL)
                throw new PaymentException(ResourceExceptMessages.CURRENCY_NOT_ALLOWED, System.Net.HttpStatusCode.Unauthorized);

            var payment = new PaymentEntity()
            {
                Amount = userOrder.TotalPrice,
                Currency = CurrencyEnum.BRL,
                CustomerId = userId,
                PaymentMethodType = PaymentMethodEnum.Pix,
            };

            var sendPayment = await _pixService.ProcessPixTransaction(user.cpf, user.email, request.FirstName, request.LastName, payment.Amount);
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
                    statusTransaction = TransactionStatusEnum.Canceled;
                    break;
            }

            var transaction = new Transaction()
            {
                Amount = userOrder.TotalPrice,
                Currency = CurrencyEnum.BRL,
                OrderId = userOrder.Id,
                TransactionStatus = statusTransaction
            };

            await _uof.transactionWrite.Add(transaction);
            await _uof.paymentWrite.Add(payment);

            await _uof.Commit();

            var response = _mapper.Map<PaymentPixResponse>(sendPayment);
            response.GatewayId = sendPayment.Id;
            response.TransactionId = transaction.Id;
            response.PaymentId = payment.Id;

            return response;
        }

        string MaskCpf(string cpf) => string.Concat(cpf.AsSpan(0, 3), ".***.***-**");
    }
}
