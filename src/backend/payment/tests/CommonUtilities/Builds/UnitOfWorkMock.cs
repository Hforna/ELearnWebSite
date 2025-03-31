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

            mock.Setup(d => d.orderRead).Returns(orderRead);

            mock.Setup(d => d.orderWrite).Returns(WriteRepositoriesMock.BuildOrder());
            mock.Setup(d => d.paymentWrite).Returns(WriteRepositoriesMock.BuildPayment());
            mock.Setup(d => d.transactionWrite).Returns(WriteRepositoriesMock.BuildTransaction());
            mock.Setup(d => d.balanceWrite).Returns(WriteRepositoriesMock.BuildBalance());

            return mock.Object;
        }
    }
}
