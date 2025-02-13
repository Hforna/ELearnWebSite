using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IModuleReadOnly
    {
        public Task<Module?> ModuleById(long id);
        public Task<List<Module>?> ModulesByCourseId(long courseId);
        public Task<Module?> ModuleByCourseAndModuleIds(long courseId, long moduleId);
        public Task<IList<Module>> GetNotActiveModules();
    }
}
