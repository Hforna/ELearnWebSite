using Bogus;
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
        public List<long> GetMostPopularCourses(long courseId, int visits)
        {
            var faker = new Faker<Dictionary<long, int>>()
            .CustomInstantiator(
            f =>
            {
                var dict = new Dictionary<long, int>();
                for(long i = 0; i < 10; i++)
                {
                    dict[i] = f.Random.Int();
                }
                return dict;
            });

            var generate = faker.Generate();
            generate[courseId] = visits;

            _mock.Setup(d => d.GetMostPopularCourses()).ReturnsAsync(generate);

            return generate.Select(d => d.Key).ToList();
        }
    } 
}
