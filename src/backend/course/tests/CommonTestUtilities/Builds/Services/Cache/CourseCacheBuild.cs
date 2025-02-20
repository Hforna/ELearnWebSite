using Course.Domain.Cache;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.Cache
{
    public class CourseCacheBuild
    {
        private readonly Mock<ICourseCache> _mock = new Mock<ICourseCache>();

        public ICourseCache Build() => _mock.Object;
        public void GetMostPopularCourses(Dictionary<long, int> courses)
        {
            _mock.Setup(d => d.GetMostPopularCourses()).ReturnsAsync(courses);
        }
    } 
}
