using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class ModulesResponse
    {
        public string CourseId { get; set; }
        public List<ModuleResponse> Modules { get; set; }
    }
}
