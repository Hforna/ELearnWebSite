using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class RestException : BaseException
    {
        public override List<string> Errors { get; set; } = [];

        public RestException(string error) : base(error) => Errors.Add(error);
        public RestException(List<string> errors) : base(string.Empty) => Errors = errors;
    }
}
