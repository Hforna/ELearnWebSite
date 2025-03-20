using Bogus;
using CommonUtilities.Builds;
using Payment.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Fakers.DTOs
{
    public class CourseDtoFaker
    {
        public CourseDto Build()
        {
            var price = (double)new Faker().Finance.Amount(20, 2000, 2);
            return new Faker<CourseDto>()
                .RuleFor(d => d.price, f => price)
                .RuleFor(d => d.description, f => f.Lorem.Paragraphs())
                .RuleFor(d => d.duration, f => (double)f.Finance.Amount(2, 20, 2))
                .RuleFor(d => d.id, SqidsMock.GenerateRandomSqid())
                .RuleFor(d => d.teacherId, SqidsMock.GenerateRandomSqid())
                .RuleFor(d => d.title, f => f.Commerce.ProductName());
        }
    }
}
