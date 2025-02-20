using CommonTestUtilities.Builds.DTOs;
using Course.Domain.Services.Rest;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services
{
    public class UserServiceBuild
    {
        private readonly Mock<IUserService> _mock = new Mock<IUserService>();

        public IUserService Build(bool isUserNull = false)
        {
            _mock.Setup(d => d.GetUserInfos()).ReturnsAsync(isUserNull ? null : new UserInfoDtoTest().Build());
            return _mock.Object;
        }
    }
}
