using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Repositories.Modules
{
    public interface IDeleteModule
    {
        public Task Execute(long id);
    }
}
