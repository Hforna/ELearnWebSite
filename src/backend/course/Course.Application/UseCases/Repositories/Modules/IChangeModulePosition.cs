using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Modules
{
    public interface IChangeModulePosition
    {
        public Task<ModulesResponse> Execute(long courseId, long moduleId, int position);
    }
}
