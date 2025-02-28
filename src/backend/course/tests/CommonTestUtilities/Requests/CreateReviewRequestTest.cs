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
    public class CreateReviewRequestTest
    {
        public CreateReviewRequest Build(int numParagraphs)
        {
            return new Faker<CreateReviewRequest>()
                .RuleFor(d => d.Rating, f => f.PickRandom<CourseRatingEnum>())
                .RuleFor(d => d.Text, f => f.Lorem.Paragraph(numParagraphs));
        }
    }
}
