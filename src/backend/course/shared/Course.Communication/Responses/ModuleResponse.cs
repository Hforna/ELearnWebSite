using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class ModuleResponse : BaseResponse
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Position { get; set; }
    }
}
