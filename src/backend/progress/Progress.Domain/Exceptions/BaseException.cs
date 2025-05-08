using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Exceptions
{
    public abstract class BaseException : SystemException
    {
        public abstract List<string> Errors { get; set; }
        public abstract HttpStatusCode StatusCode { get; set; }

        public abstract HttpStatusCode GetStatusCode();
        public abstract string GetMessage();
    }
}
