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

        public RestException(List<string> errors) => Errors = errors;
        public RestException(string error) => Errors.Add(error);

        public override IList<string> GetMessage() => [Message];

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
    }
}
