using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IModuleWriteOnly
    {
        public void AddModule(Module module);
        public void UpdateModule(Module module);
        public void UpdateModules(List<Module> modules);
        public void DeleteModule(Module module);
        public void DeactiveModule(Module module);
    }
}
