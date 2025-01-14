using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class UserException : BaseException
    {
        public override List<string> Errors { get; set; } = [];

        public UserException(List<string> errors) => Errors = errors;
        public UserException(string error) => Errors.Add(error);

        public override string GetMessage() => Message;

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.NotFound;
    }
}
