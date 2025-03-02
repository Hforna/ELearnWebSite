using Bogus;
using Course.Communication.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Requests
{
    public class UpdateModuleRequestTest
    {
        public static UpdateModuleRequest Build(int position)
        {
            return new Faker<UpdateModuleRequest>()
                .RuleFor(d => d.Name, f => f.Lorem.Word())
                .RuleFor(d => d.Description, f => f.Lorem.Paragraph(5))
                .RuleFor(d => d.Position, f => position);
        }
    }
}
