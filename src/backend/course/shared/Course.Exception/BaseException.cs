using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public abstract class BaseException : SystemException
    {
        public abstract List<string> Errors { get; set; }

        public BaseException(List<string> errors) : base(string.Empty)
        {
            Errors = errors;
        }

        public BaseException(string error) : base(error) => Errors = new List<string>() { error };

        public List<string> GetMessage() => Errors;
    }
}
