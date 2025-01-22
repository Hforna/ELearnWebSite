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

        public JsonErrorResponse(List<string> errors) => Errors = errors;

        public JsonErrorResponse(string error) => Errors.Add(error);

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;

        public override string GetMessage() => Message;
    }
}
