using Moq;
using Payment.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Repositories.Commands
{
    public static class OrderWriteOnlyMock
    {
        public static IOrderWriteOnly Build() => new Mock<IOrderWriteOnly>().Object;
    }
}
