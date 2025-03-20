using Moq;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Services.Rest
{
    public class UserRestServiceMock
    {
        private readonly Mock<IUserRestService> _mock = new Mock<IUserRestService>();

        public IUserRestService Build() => _mock.Object;

    }
}
