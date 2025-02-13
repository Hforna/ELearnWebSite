using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class JsonErrorResponse : BaseException
    {
        public override List<string> Errors { get; set; } = [];
        public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

        public JsonErrorResponse(List<string> errors, HttpStatusCode? statusCode = null)
        {
            Errors = errors;
            if (statusCode.HasValue)
                StatusCode = (HttpStatusCode)statusCode;
        }

        public JsonErrorResponse(string error, HttpStatusCode? statusCode = null)
        {
            Errors.Add(error);
            if (statusCode.HasValue)
                statusCode = (HttpStatusCode)statusCode;
        }

        public override HttpStatusCode GetStatusCode() => StatusCode;

        public override string GetMessage() => Message;
    }
}
