using CommonUtilities.Fakers.DTOs;
using Moq;
using Payment.Domain.DTOs;
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

        public void GetCourse(CourseDto courseDto)
        {
            _mock.Setup(d => d.GetCourse(courseDto.id)).ReturnsAsync(courseDto);
        }
    }
}
