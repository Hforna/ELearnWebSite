using Bogus;
using CommonUtilities.Builds;
using Payment.Application.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Fakers.Requests
{
    public class AddCourseToOrderRequestFaker
    {
        public AddCourseToOrderRequest Build()
        {
            var generateId = SqidsMock.GenerateRandomSqid();

            return new Faker<AddCourseToOrderRequest>()
                .RuleFor(d => d.CourseId, generateId);
        }
    }
}
