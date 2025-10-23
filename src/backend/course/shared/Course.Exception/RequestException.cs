using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class RequestException : BaseException
    {
        public override List<string> Errors { get; set; } = [];

        public RequestException(string error) => Errors.Add(error);
        public RequestException(List<string> errors) => Errors = errors;
    }
}
