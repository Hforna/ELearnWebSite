using Course.Domain.Services.RabbitMq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.RabbitMq
{
    public static class UserSenderServiceBuild
    {
        public static IUserSenderService Build()
        {
            var mock = new Mock<IUserSenderService>();

            return mock.Object;
        }
    }
}
