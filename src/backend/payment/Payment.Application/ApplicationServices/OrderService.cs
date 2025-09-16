using AutoMapper;
using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Extensions;
using Payment.Application.Requests;
using Payment.Application.Responses.Order;
using Payment.Domain.Cons;
using Payment.Domain.DTOs;
using Payment.Domain.Entities;
using Payment.Domain.Enums;
using Payment.Domain.Exceptions;
using Payment.Domain.Repositories;
using Payment.Domain.Services.Rest;
using Payment.Domain.Services.Session;
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
        private readonly IOrderSessionService _orderSession;

        public OrderService(IUnitOfWork uow, SqidsEncoder<long> sqids, ICourseRestService courseRest, 
            IUserRestService userRest, IMapper mapper, 
            ILogger<OrderService> logger, ICurrencyExchangeService currencyExchange, 
            ILocationRestService locationRest, IOrderSessionService orderSession)
        {
            _courseRest = courseRest;
            _uow = uow;
            _orderSession = orderSession;
            _locationRest = locationRest;
            _currencyExchange = currencyExchange;
            _userRest = userRest;
            _mapper = mapper;
            _sqids = sqids;
            _logger = logger;
        }

        public async Task<OrderItemResponse> AddCourseToOrder(AddCourseToOrderRequest request)
        {
            var course = await _courseRest.GetCourse(request.CourseId);
            var courseId = _sqids.Decode(course.id).Single();

            var currencyExchange = await _currencyExchange.GetCurrencyRates(course.currencyType);
            var userCurrencyAsEnum = await UserCurrencyAsEnumExtension.GetCurrency(_locationRest);
            var priceResponse = (double)GetCurrencyRate(userCurrencyAsEnum, currencyExchange)! * course.price;

            var response = new OrderItemResponse();

            try
            {
                var user = await _userRest.GetUserInfos();
                var userId = _sqids.Decode(user.id).Single();

                var orderItemExists = await _uow.orderRead.OrderItemExists(courseId, userId);

                if (orderItemExists)
                    throw new OrderException(ResourceExceptMessages.ORDER_ITEM_EXISTS, System.Net.HttpStatusCode.BadRequest);

                var userOrder = await _uow.orderRead.OrderByUserId(userId);

                if (userOrder is null)
                {
                    userOrder = new Order() { UserId = userId };
                    await _uow.orderWrite.AddOrder(userOrder);
                    await _uow.Commit();
                }
                userOrder.TotalPrice += (decimal)course.price;

                var orderItem = new OrderItem()
                {
                    Price = (decimal)course.price,
                    CourseId = courseId,
                    OrderId = userOrder.Id
                };

                _mapper.Map(orderItem, response);

                await _uow.orderWrite.AddOrderItem(orderItem);
                await _uow.Commit();
            } catch(RestException re)
            {
                _orderSession.AddOrderToSession(courseId);

                response.CourseId = course.id;
            }
            response.CurrencyType = DefaultCurrency.Currency;
            response.Price = (decimal)priceResponse;

            return response;
        }

        public async Task<OrderHistoryResponse> GetOrderHistory(int page, int quantity)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var orderHistory = _uow.orderRead.GetOrdersNotActive(page, quantity, userId);

            if (orderHistory is null)
                throw new OrderException(ResourceExceptMessages.ORDERS_DONT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var userCurrency = await UserCurrencyAsEnumExtension.GetCurrency(_locationRest);
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
            var userCurrencyAsEnum = await UserCurrencyAsEnumExtension.GetCurrency(_locationRest);

            var currencyExchange = await _currencyExchange.GetCurrencyRates(userCurrencyAsEnum);

            var response = new OrderResponse();
            try
            {
                var user = await _userRest.GetUserInfos();
                var userId = _sqids.Decode(user.id).Single();

                _logger.LogInformation($"user id: {userId}");

                var order = await _uow.orderRead.OrderByUserId(userId);

                if (order is null)
                    throw new OrderException(ResourceExceptMessages.ORDER_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

                response = _mapper.Map<OrderResponse>(order);

                var rate = GetCurrencyRate(DefaultCurrency.Currency, currencyExchange);

                response.OrderItems = order.OrderItems.Select(orderItem =>
                {
                    var response = _mapper.Map<OrderItemResponse>(orderItem);
                    response.Price *= rate;

                    return response;
                }).ToList();
            } catch(RestException re)
            {
                var sessionOrder = _orderSession.GetSessionOrder();

                var orderItemResponse = sessionOrder.Select(async order =>
                {
                    var course = await _courseRest.GetCourse(_sqids.Encode(order.Value));
                    var response = new OrderItemResponse()
                    {
                        Id = order.Key,
                        CurrencyType = course.currencyType,
                        CourseId = course.id,
                        Price = (decimal)course.price
                    };

                    return response;
                });

                var processOrderItems = await Task.WhenAll(orderItemResponse);

                response.OrderItems = processOrderItems.ToList();
            }

            return response;
        }

        public async Task RemoveCourseFromOrder(long courseId)
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var userCurrency = await UserCurrencyAsEnumExtension.GetCurrency(_locationRest);
            var currencyRates = await _currencyExchange.GetCurrencyRates(userCurrency);

            var order = await _uow.orderRead.OrderByUserId(userId) 
                ?? throw new OrderException(ResourceExceptMessages.ORDERS_DONT_EXISTS);

            var courseOrderItem = await _uow.orderRead.GetCourseAndUserOrderItem(userId, courseId)
                ?? throw new OrderException(ResourceExceptMessages.COURSE_IS_NOT_IN_ORDER);

            var course = await _courseRest.GetCourse(_sqids.Encode(courseId));

            order.TotalPrice -= (decimal)course!.price;

            _uow.orderWrite.DeleteOrderItem(courseOrderItem);
            _uow.orderWrite.UpdateOrder(order);
            await _uow.Commit();
        }

        decimal GetCurrencyRate(CurrencyEnum currency, RateExchangeDto rateExchange)
        {
            switch(currency)
            {
                case CurrencyEnum.USD:
                    return rateExchange.USD;
                case CurrencyEnum.EUR:
                    return rateExchange.EUR;
                case CurrencyEnum.BRL:
                    return rateExchange.BRL;
                default:
                    return 0;
            }
        }
    }
}
