using CommonUtilities.Builds.Mapper;
using CommonUtilities.Builds.Repositories.OrderMock;
using CommonUtilities.Builds.Services.Rest;
using CommonUtilities.Builds;
using CommonUtilities.Fakers.Entities;
using Payment.Application.Services;
using Payment.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonUtilities.Fakers.DTOs;
using FluentAssertions;

namespace UnitTests.Services
{
    public class GetUserOrderTest
    {
        [Fact]
        public async Task Success()
        {
            var userInfos = new UserInfoDtoFaker().Build();
            var userId = SqidsMock.Build().Decode(userInfos.id).Single();
            var orderService = CreateService(userInfos, userId);
            var result = await orderService.GetUserOrder();

            result.OrderItems.Should().NotBeNullOrEmpty();
            result.UserId.Should().Be(userInfos.id);
        }

        public OrderService CreateService(UserInfoDto userInfoDto,long userId)
        {
            //entities
            var order = OrderEntityFaker.Build(userId);
            //repositories
            var orderRead = new OrderReadOnlyMock();
            orderRead.OrderByUser(order);

            var uow = UnitOfWorkMock.Build(orderRead.Build());

            var sqids = SqidsMock.Build();
            var courseRest = new CourseRestServiceMock();

            var userRest = new UserRestServiceMock();
            userRest.GetUserInfos(userInfoDto);

            var mapper = AutoMapperMock.Build();

            return new OrderService(uow, sqids, courseRest.Build(), userRest.Build(), mapper);
        }
    }
}
