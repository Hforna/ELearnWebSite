using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class CreateLessonRequest
    {
        public string Title { get; set; }
        public bool isFree { get; set; }
        public int Order { get; set; }
        public IFormFile Video { get; set; }
    }
}
