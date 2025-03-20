using CommonUtilities.Builds;
using CommonUtilities.Builds.Mapper;
using CommonUtilities.Builds.Services.Rest;
using CommonUtilities.Fakers.DTOs;
using CommonUtilities.Fakers.Requests;
using Payment.Application.Services;
using Payment.Domain.DTOs;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidator;
using FluentAssertions;
using CommonUtilities.Fakers.Entities;
using CommonUtilities.Builds.Repositories.OrderMock;
using Payment.Domain.Entities;
using Payment.Domain.Exceptions;

namespace UnitTests.Services
{
    public class AddCourseToOrderTest
    {
        [Fact]
        public async Task Success()
        {
            var request = new AddCourseToOrderRequestFaker().Build();
            var userInfos = new UserInfoDtoFaker().Build();
            var userId = SqidsMock.Build().Decode(userInfos.id).Single();
            var courseDto = new CourseDtoFaker().Build();
            courseDto.id = request.CourseId; 
            var courseId = SqidsMock.Build().Decode(courseDto.id).Single();

            var orderService = CreateService(userInfos, courseDto, courseId, userId);
            var result = await orderService.AddCourseToOrder(request);

            result.CourseId.Should().Be(request.CourseId);
        }

        [Fact]
        public async Task ErrorOrderItemExists()
        {
            var request = new AddCourseToOrderRequestFaker().Build();
            var userInfos = new UserInfoDtoFaker().Build();
            var userId = SqidsMock.Build().Decode(userInfos.id).Single();
            var courseDto = new CourseDtoFaker().Build();
            courseDto.id = request.CourseId;
            var courseId = SqidsMock.Build().Decode(courseDto.id).Single();

            var orderService = CreateService(userInfos, courseDto, courseId, userId, true);
            var result = async () => await orderService.AddCourseToOrder(request);

            await result.Should().ThrowAsync<OrderException>(ResourceExceptMessages.ORDER_ITEM_EXISTS);
        }

        public OrderService CreateService(UserInfoDto userInfoDto, CourseDto course, long courseId, long userId, bool orderItemExists = false)
        {
            //entities
            var order = OrderEntityFaker.Build(userId);
            //repositories
            var orderRead = new OrderReadOnlyMock();
            orderRead.OrderItemExists(courseId, userId, orderItemExists);
            orderRead.OrderByUser(order);

            var uow = UnitOfWorkMock.Build(orderRead.Build());

            var sqids = SqidsMock.Build();
            var courseRest = new CourseRestServiceMock();
            courseRest.GetCourse(course);


            var userRest = new UserRestServiceMock();
            userRest.GetUserInfos(userInfoDto);

            var mapper = AutoMapperMock.Build();

            return new OrderService(uow, sqids, courseRest.Build(), userRest.Build(), mapper);
        }
    }
}
