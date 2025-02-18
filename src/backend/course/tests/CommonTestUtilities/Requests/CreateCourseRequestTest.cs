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
    public class CreateCourseRequestTest
    {
        public CreateCourseRequest Build()
        {
            return new Faker<CreateCourseRequest>()
                .RuleFor(d => d.Title, x => x.Company.CompanyName())
                .RuleFor(d => d.Description, x => x.Lorem.Paragraph())
                .RuleFor(d => d.CourseLanguage, x => x.PickRandom<LanguagesEnum>())
                .RuleFor(d => d.Price, x => (double)x.Finance.Amount(20, 500, 2))
                .RuleFor(d => d.CourseTopics, x => x.Make<CreateCourseTopicsRequest>(5, f => new CreateCourseTopicsRequest() { Topic = x.Random.Words() }))
                .RuleFor(d => d.TypeCourse, x => x.PickRandom<TypeCourseEnum>());
        }
    }
}
