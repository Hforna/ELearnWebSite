using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Requests
{
    public class CreateModuleRequest
    {
        public string Title { get; set; }
        public int? Position { get; set; }
        public bool? moveToFront { get; set; }
        public string Description { get; set; }
    }
}
