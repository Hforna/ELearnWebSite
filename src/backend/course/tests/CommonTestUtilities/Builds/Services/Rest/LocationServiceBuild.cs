using Course.Domain.DTOs;
using Course.Domain.Services.Rest;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.Rest
{
    public class LocationServiceBuild
    {
        private readonly Mock<ILocationService> _mock = new Mock<ILocationService>();
        public ILocationService Build() => _mock.Object;

        public void CurrencyByUserLocation(CurrencyByLocationDto locationDto)
        {
            _mock.Setup(d => d.GetCurrencyByUserLocation()).ReturnsAsync(locationDto);
        }
    }
}
