using Course.Domain.Entitites;
using Course.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data
{
    public class ModuleRepository : IModuleReadOnly, IModuleWriteOnly
    {
        private readonly CourseDbContext _dbContext;

        public ModuleRepository(CourseDbContext dbContext) => _dbContext = dbContext;

        public void AddModule(Module module)
        {
            _dbContext.Modules.Add(module);
        }

        public void UpdateModule(Module module)
        {
            _dbContext.Modules.Update(module);
        }

        public void UpdateModules(List<Module> modules)
        {
            _dbContext.Modules.UpdateRange(modules);
        }
    }
}
