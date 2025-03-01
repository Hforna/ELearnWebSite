using Bogus;
using Course.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Requests
{
    public static class CreateModuleRequestTest
    {
        public static CreateModuleRequest Build(int position)
        {
            return new Faker<CreateModuleRequest>()
                .RuleFor(d => d.Description, f => f.Lorem.Paragraphs(5))
                .RuleFor(d => d.Name, f => f.Lorem.Word())
                .RuleFor(d => d.Position, position);

        }
    }
}
