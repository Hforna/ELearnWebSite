using Bogus;
using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Entities
{
    public class EnrollmentEntityTest
    {
        public static Enrollment Build(CourseEntity course)
        {
            return new Faker<Enrollment>()
                .RuleFor(d => d.CustomerId, f => f.Random.Long(1, 5000))
                .RuleFor(d => d.Active, f => true)
                .RuleFor(d => d.CourseId, f => course.Id)
                .RuleFor(d => d.Course, f => course)
                .RuleFor(d => d.Id, f => f.Random.Long(1, 5000));
        }

        public static List<Enrollment> BuildEnrollmentRange(CourseEntity course)
        {
            var enrollments = new List<Enrollment>();

            for(var i = 0; i < 20; i++)
            {
                var enrollment = new Faker<Enrollment>()
                    .RuleFor(d => d.CustomerId, f => f.Random.Long(1, 5000))
                    .RuleFor(d => d.Active, f => true)
                    .RuleFor(d => d.CourseId, f => course.Id)
                    .RuleFor(d => d.Course, f => course)
                    .RuleFor(d => d.Id, f => i)
                    .Generate();

                enrollments.Add(enrollment);
            }

            return enrollments;
        }
    } 
}
