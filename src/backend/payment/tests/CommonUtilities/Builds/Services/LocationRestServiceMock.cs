using Moq;
using Payment.Domain.DTOs;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Services
{
    public class LocationRestServiceMock
    {
        private readonly Mock<ILocationRestService> _mock = new Mock<ILocationRestService>();

        public ILocationRestService Build() => _mock.Object;

        public void CurrencyByUserLocation(CurrencyByLocationDto locationDto) => _mock.Setup(d => d.GetCurrencyByUserLocation()).ReturnsAsync(locationDto);
            
    }
}
