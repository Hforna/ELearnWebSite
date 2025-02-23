using Course.Domain.Services.Rest;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.Rest
{
    public static class LinkServiceBuild
    {
        public static ILinkService Build(string? routePath = "", string? routeValues = "")
        {
            var mock = new Mock<ILinkService>();
            mock.Setup(d => d.GenerateResourceLink(routePath!, routeValues!)).Returns($"https://app/{routePath}/{routeValues}");

            return mock.Object;
        }
    }
}
