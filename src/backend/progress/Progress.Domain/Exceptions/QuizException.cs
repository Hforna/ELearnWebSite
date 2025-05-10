using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Exceptions
{
    public class QuizException : BaseException
    {
        public override List<string> Errors { get; set; } = [];
        public override HttpStatusCode StatusCode { get; set; }

        public QuizException(List<string> errors, HttpStatusCode statusCode)
        {
            Errors = errors;
            StatusCode = statusCode;
        }

        public QuizException(string error, HttpStatusCode statusCode)
        {
            Errors.Add(error);
            StatusCode = statusCode;
        }

        public override HttpStatusCode GetStatusCode() => StatusCode;

        public override string GetMessage() => Message;
    }
}
