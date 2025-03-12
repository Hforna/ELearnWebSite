using Course.Communication.Requests;
using Course.Communication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Modules
{
    public interface ICreateModule
    {
        public Task<IList<ModuleResponse>> Execute(CreateModuleRequest request, long courseId);
    }
}
