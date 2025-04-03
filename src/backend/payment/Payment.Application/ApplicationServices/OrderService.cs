using AutoMapper;
using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Application.Responses.Order;
using Payment.Domain.Cons;
using Payment.Domain.DTOs;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Repositories;
using Payment.Domain.Services.Rest;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly SqidsEncoder<long> _sqids;
        private readonly IMapper _mapper;
        private readonly ICourseRestService _courseRest;
        private readonly IUserRestService _userRest;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<OrderService> _logger;
        private readonly ICurrencyExchangeService _currencyExchange;
        private readonly ILocationRestService _locationRest;

        public OrderService(IUnitOfWork uow, SqidsEncoder<long> sqids, ICourseRestService courseRest, 
            IUserRestService userRest, IMapper mapper, 
            ILogger<OrderService> logger, ICurrencyExchangeService currencyExchange, ILocationRestService locationRest)
        {
            _courseRest = courseRest;
            _uow = uow;
            _locationRest = locationRest;
            _currencyExchange = currencyExchange;
            _userRest = userRest;
            _mapper = mapper;
            _sqids = sqids;
            _logger = logger;
        }

        public async Task<OrderItemResponse> AddCourseToOrder(AddCourseToOrderRequest request)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _courseRest.GetCourse(request.CourseId);
            var courseId = _sqids.Decode(course.id).Single();

            var orderItemExists = await _uow.orderRead.OrderItemExists(courseId, userId);

            if (orderItemExists)
                throw new OrderException(ResourceExceptMessages.ORDER_ITEM_EXISTS, System.Net.HttpStatusCode.BadRequest);

            var userOrder = await _uow.orderRead.OrderByUserId(userId);

            if(userOrder is null)
            {
                userOrder = new Order() { UserId = userId };
                await _uow.orderWrite.AddOrder(userOrder);
                await _uow.Commit();
            }
            userOrder.TotalPrice += (decimal)course.price;
            var currencyExchange = await _currencyExchange.GetCurrencyRates(course.currencyType);
            switch(DefaultCurrency.Currency)
            {
                case CurrencyEnum.BRL:
                    course.price *= currencyExchange.BRL;
                    break;
                case CurrencyEnum.USD:
                    course.price *= currencyExchange.USD;
                    break;
                case CurrencyEnum.EUR:
                    course.price *= currencyExchange.EUR;
                default:
                    break;
            }

            var orderItem = new OrderItem()
            {
                Price = (decimal)course.price,
                CourseId = courseId,
                OrderId = userOrder.Id
            };

            await _uow.orderWrite.AddOrderItem(orderItem);
            await _uow.Commit();

            var response = _mapper.Map<OrderItemResponse>(orderItem);

            return response;
        }

        public async Task<OrderHistoryResponse> GetOrderHistory(int page, int quantity)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var orderHistory = _uow.orderRead.GetOrdersNotActive(page, quantity, userId);

            if (orderHistory is null)
                throw new OrderException(ResourceExceptMessages.ORDERS_DONT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var getUserCurrency = await _locationRest.GetCurrencyByUserLocation();
            var userCurrency = Enum.TryParse(typeof(CurrencyEnum), getUserCurrency.Code, out var result)
                ? (CurrencyEnum)result
                : DefaultCurrency.Currency;
            var currencyExchange = await _currencyExchange.GetCurrencyRates(userCurrency);

            var transactions = await _uow.transactionRead.TransactionsByOrderIds(orderHistory.Select(d => d.Id).ToList());
            var dictTransactions = transactions.ToDictionary(d => d.OrderId);

            var orderResponse = _mapper.Map<List<OrderShortResponse>>(orderHistory);
            var selectOrder = orderResponse.Select(order =>
            {
                var transaction = dictTransactions[order.Id];

                order.PurchaseDate = transaction.TransactionStatus == TransactionStatusEnum.Approved ? transaction.UpdatedOn : null;
                order.Status = transaction.TransactionStatus;
                order.TotalPrice *= (decimal)GetCurrencyRate(order.Currency, currencyExchange)!;

                return order;
            });

            var response = new OrderHistoryResponse()
            {
                Orders = selectOrder.ToList(),
                Count = orderHistory.Count,
                IsFirstPage = orderHistory.IsFirstPage,
                IsLastPage = orderHistory.IsLastPage,
                PageNumber = orderHistory.PageNumber,
                TotalItemCount = orderHistory.TotalItemCount
            };

            return response;
        }

        public async Task<OrderResponse> GetUserOrder()
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            _logger.LogInformation($"user id: {userId}");

            var order = await _uow.orderRead.OrderByUserId(userId);

            if (order is null)
                throw new OrderException(ResourceExceptMessages.ORDER_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var response = _mapper.Map<OrderResponse>(order);

            response.OrderItems = order.OrderItems.Select(orderItem =>
            {
                var response = _mapper.Map<OrderItemResponse>(orderItem);

                return response;
            }).ToList();

            return response;
        }

        decimal? GetCurrencyRate(CurrencyEnum currency, RateExchangeDto rateExchange)
        {
            switch(currency)
            {
                case CurrencyEnum.USD:
                    return (decimal)rateExchange.USD;
                case CurrencyEnum.EUR:
                    return (decimal)rateExchange.EUR;
                case CurrencyEnum.BRL:
                    return (decimal)rateExchange.BRL;
                default:
                    return null;
            }
        }
    }
}
