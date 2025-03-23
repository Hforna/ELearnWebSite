using AutoMapper;
using Microsoft.Extensions.Logging;
using Payment.Application.ApplicationServices.Interfaces;
using Payment.Application.Requests;
using Payment.Application.Responses;
using Payment.Domain.Cons;
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
        private readonly ICurrencyExchangeService _exchangeService;
        private readonly ILocationRestService _locationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork uow, SqidsEncoder<long> sqids, ICourseRestService courseRest, 
            IUserRestService userRest, IMapper mapper, 
            ICurrencyExchangeService exchangeService, ILocationRestService locationService, ILogger<OrderService> logger)
        {
            _courseRest = courseRest;
            _uow = uow;
            _exchangeService = exchangeService;
            _locationService = locationService;
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

        public async Task<OrderResponse> GetUserOrder()
        {
            var user = await _userRest.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var order = await _uow.orderRead.OrderByUserId(userId);

            if (order is null)
                throw new OrderException(ResourceExceptMessages.ORDER_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var getUserCurrency = await _locationService.GetCurrencyByUserLocation();
            _logger.LogInformation($"user current currency: {getUserCurrency.Code}");

            var userCurrencyCode = Enum.GetNames<CurrencyEnum>().ToList().Contains(getUserCurrency.Code) 
                ? getUserCurrency.Code.ToString() 
                : DefaultCurrency.Currency.ToString();

            var response = _mapper.Map<OrderResponse>(order);

            var rateCurrency = await _exchangeService.GetCurrencyRates((CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), userCurrencyCode));

            response.OrderItems = order.OrderItems.Select(orderItem =>
            {
                var response = _mapper.Map<OrderItemResponse>(orderItem);
                response.CurrencyType = userCurrencyCode;
                //response.Price = 

                return response;
            }).ToList();

            return response;
        }
    }
}
