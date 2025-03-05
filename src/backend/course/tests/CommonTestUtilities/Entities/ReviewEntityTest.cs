using Bogus;
using Course.Domain.Entitites;
using Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Entities
{
    public static class ReviewEntityTest
    {
        public static Review Build(long courseId, long userId)
        {
            return new Faker<Review>()
                .RuleFor(d => d.Rating, f => f.PickRandom<CourseRatingEnum>())
                .RuleFor(d => d.Text, f => f.Lorem.Paragraph())
                .RuleFor(d => d.Id, 1)
                .RuleFor(d => d.CourseId, courseId)
                .RuleFor(d => d.CustomerId, userId);
        }
    }
}
