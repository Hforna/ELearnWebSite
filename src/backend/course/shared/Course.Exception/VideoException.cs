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

        public VideoException(string error) => Errors.Add(error);

        public VideoException(List<string> errors) => Errors = errors;

        public override string GetMessage() => Message;

        public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
    }
}
