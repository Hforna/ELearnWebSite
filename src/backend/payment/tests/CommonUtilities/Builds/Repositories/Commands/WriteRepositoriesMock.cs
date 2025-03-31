using Moq;
using Payment.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Repositories.Commands
{
    public static class WriteRepositoriesMock
    {
        public static IOrderWriteOnly BuildOrder() => new Mock<IOrderWriteOnly>().Object;
        public static IBalanceWriteOnly BuildBalance() => new Mock<IBalanceWriteOnly>().Object;
        public static ITransactionWriteOnly BuildTransaction() => new Mock<ITransactionWriteOnly>().Object;
        public static IPaymentWriteOnly BuildPayment() => new Mock<IPaymentWriteOnly>().Object;
    }
}
