using Moq;
using Payment.Domain.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Services.Rest
{
    public class CourseRestServiceMock
    {
        private readonly Mock<ICourseRestService> _mock = new Mock<ICourseRestService>();

        public ICourseRestService Build() => _mock.Object;
    }
}
