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
    public class UpdateCourseRequestTest
    {
        public UpdateCourseRequest Build()
        {
            return new Faker<UpdateCourseRequest>()
                .RuleFor(d => d.Title, x => x.Company.CompanyName())
                .RuleFor(d => d.Description, x => x.Lorem.Paragraph())
                .RuleFor(d => d.TypeCourse, f => f.PickRandom<TypeCourseEnum>())
                .RuleFor(d => d.TopicsCovered, f => f.Make<CreateCourseTopicsRequest>(5, s => new CreateCourseTopicsRequest() { Topic = f.Commerce.Product() }))
                .RuleFor(d => d.CourseLanguage, f => f.PickRandom<LanguagesEnum>())
                .RuleFor(d => d.Price, f => (double)f.Finance.Amount(20, 500, 2));
        }

    }
}
