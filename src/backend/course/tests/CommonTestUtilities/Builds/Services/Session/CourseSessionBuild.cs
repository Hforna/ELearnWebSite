using Bogus;
using Bogus.DataSets;
using Course.Domain.Sessions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.Session
{
    public static class CourseSessionBuild
    {
        public static (List<long>, ICoursesSession) Build(long courseId)
        {
            var faker = new Faker();
            List<long> nums = Enumerable.Range(1, 10)
                .Select(_ => faker.Random.Long(1, 1000))
                .ToList();

            nums.Add(courseId);
            var mock = new Mock<ICoursesSession>();
            mock.Setup(d => d.GetCoursesVisited()).Returns(nums);

            return (nums, mock.Object);
        }
    }
}
