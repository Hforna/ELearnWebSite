using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Services.Azure
{
    public interface INewModuleSender
    {
        public Task SendMessage(long moduleId);
    }
}
