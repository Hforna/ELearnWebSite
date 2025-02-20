using Bogus;
using CommonTestUtilities.Builds.Services;
using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Entities
{
    public class CourseEntityTest
    {
        public static CourseEntity Build(long? id = null)
        {
            var courseId = id is null ? SqidsBuild.GenerateRandomLong() : id;

            return new Faker<CourseEntity>()
                .RuleFor(d => d.TeacherId, SqidsBuild.GenerateRandomLong())
                .RuleFor(d => d.IsPublish, true)
                .RuleFor(d => d.Active, true)
                .RuleFor(d => d.Title, f => f.Company.CompanyName())
                .RuleFor(d => d.Description, f => f.Lorem.Paragraph())
                .RuleFor(d => d.Id, courseId)
                .RuleFor(d => d.Price, f => (double)f.Finance.Amount(10, 2000, 2))
                .RuleFor(d => d.TopicsCovered, f => f.Make<CourseTopicsEntity>(5, d => new CourseTopicsEntity()
                {
                    Active = true,
                    CourseId = (long)courseId,
                    CreatedOn = DateTime.UtcNow,
                    Id = SqidsBuild.GenerateRandomLong(),
                    Topic = f.Lorem.Word()
                }));
        }
    }
}
