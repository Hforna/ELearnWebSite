using Bogus;
using CommonTestUtilities.Builds.Repositories.Executes;
using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Entities
{
    public class ModuleEntityTest
    {
        public Collection<Module> CreateModules(int quantity, long courseId)
        {
            var modules = new Collection<Module>();

            for(var i = 0; i < quantity; i++)
            {
                var module = new Faker<Module>()
                    .RuleFor(d => d.CourseId, courseId)
                    .RuleFor(d => d.Active, true)
                    .RuleFor(d => d.CreatedOn, DateTime.UtcNow)
                    .RuleFor(d => d.Position, i)
                    .RuleFor(d => d.Id, i)
                    .RuleFor(d => d.Name, f => f.Lorem.Word());
                modules.Add(module);
            }

            return modules;
        }

    }
}
