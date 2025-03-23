using Bogus;
using CommonTestUtilities.Builds.Services;
using Course.Domain.Entitites;
using Course.Domain.Enums;
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

        public static List<CourseEntity> CreateRangeCourse(List<long>? ids = null, int generate = 5)
        {
            var courseList = new List<CourseEntity>();
            
            for(int i = 1; i < generate; i++)
            {
                var id = new Faker().Random.Long(1, 200);

                var course = new Faker<CourseEntity>()
                .RuleFor(d => d.TeacherId, SqidsBuild.GenerateRandomLong())
                .RuleFor(d => d.IsPublish, true)
                .RuleFor(d => d.Active, true)
                .RuleFor(d => d.Title, f => f.Company.CompanyName())
                .RuleFor(d => d.Description, f => f.Lorem.Paragraph())
                .RuleFor(d => d.Id, id)
                .RuleFor(d => d.Price, f => (double)f.Finance.Amount(10, 2000, 2))
                .RuleFor(d => d.CurrencyType, f => f.PickRandom<CurrencyEnum>())
                .RuleFor(d => d.TopicsCovered, f => f.Make<CourseTopicsEntity>(5, d => new CourseTopicsEntity()
                {
                    Active = true,
                    CourseId = (long)id,
                    CreatedOn = DateTime.UtcNow,
                    Id = SqidsBuild.GenerateRandomLong(),
                    Topic = f.Lorem.Word()
                }));

                courseList.Add(course);
            }

            for(int i = 1; i < ids.Count; i++)
            {
                var courses = new Faker<CourseEntity>()
                .RuleFor(d => d.TeacherId, SqidsBuild.GenerateRandomLong())
                .RuleFor(d => d.IsPublish, true)
                .RuleFor(d => d.Active, true)
                .RuleFor(d => d.Title, f => f.Company.CompanyName())
                .RuleFor(d => d.Description, f => f.Lorem.Paragraph())
                .RuleFor(d => d.Id, ids[i])
                .RuleFor(d => d.Price, f => (double)f.Finance.Amount(10, 2000, 2))
                .RuleFor(d => d.TopicsCovered, f => f.Make<CourseTopicsEntity>(5, d => new CourseTopicsEntity()
                {
                    Active = true,
                    CourseId = (long)ids[i],
                    CreatedOn = DateTime.UtcNow,
                    Id = SqidsBuild.GenerateRandomLong(),
                    Topic = f.Lorem.Word()
                }));
            }

            return courseList;
        }
    }
}
