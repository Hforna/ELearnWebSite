using CommonUtilities.Builds;
using CommonUtilities.Builds.Mapper;
using CommonUtilities.Builds.Services.Rest;
using Payment.Application.Services;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    public class AddUserToOrderTest
    {
        [Fact]
        public async Task Success()
        {

        }

        public OrderService CreateService()
        {
            var uow = UnitOfWorkMock.Build();
            var sqids = SqidsMock.Build();
            var courseRest = new CourseRestServiceMock();
            var userRest = new UserRestServiceMock();
            var mapper = AutoMapperMock.Build();

            return new OrderService(uow, sqids, courseRest.Build(), userRest.Build(), mapper);
        }
    }
}
