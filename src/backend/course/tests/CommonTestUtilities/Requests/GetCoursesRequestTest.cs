using Bogus;
using Course.Communication.Enums;
using Course.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Requests
{
    public class GetCoursesRequestTest
    {
        public GetCoursesRequest Build()
        {
            return new Faker<GetCoursesRequest>()
                .RuleFor(d => d.Ratings, f => f.Make(1, d => f.PickRandom<CourseRatingEnum>()))
                .RuleFor(d => d.Languages, f => f.Make(2, d => f.PickRandom<LanguagesEnum>()))
                .RuleFor(d => d.Price, f => f.PickRandom<PriceEnum>());
        }
    }
}
