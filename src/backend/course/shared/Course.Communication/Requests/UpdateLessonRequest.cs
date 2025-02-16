using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class UpdateLessonRequest
    {
        public string ModuleId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public bool isFree { get; set; }
    }
}
