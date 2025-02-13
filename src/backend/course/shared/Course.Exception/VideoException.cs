using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class VideoException : BaseException
    {
        public override List<string> Errors { get; set; } = [];
        public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

        public VideoException(List<string> errors, HttpStatusCode? statusCode = null)
        {
            Errors = errors;
            if (statusCode.HasValue)
                StatusCode = (HttpStatusCode)statusCode;
        }

        public VideoException(string error, HttpStatusCode? statusCode = null)
        {
            Errors.Add(error);
            if (statusCode.HasValue)
                statusCode = (HttpStatusCode)statusCode;
        }

        public override string GetMessage() => Message;

        public override HttpStatusCode GetStatusCode() => StatusCode;
    }
}
