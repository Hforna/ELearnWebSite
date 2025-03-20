using CommonUtilities.Builds.Repositories.Commands;
using Moq;
using Payment.Domain.Repositories;
using Payment.Infrastructure.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds
{
    public static class UnitOfWorkMock
    {
        public static IUnitOfWork Build(IOrderReadOnly? orderRead)
        {
            var mock = new Mock<IUnitOfWork>();
            mock.Setup(d => d.orderWrite).Returns(OrderWriteOnlyMock.Build());
            mock.Setup(d => d.orderRead).Returns(orderRead);

            return mock.Object;
        }
    }
}
