using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class NotAuthenticatedException : BaseException
    {
        public override List<string> Errors { get; set; } = [];

        public NotAuthenticatedException(string error) => Errors.Add(error);
        public NotAuthenticatedException(List<string> errors) => Errors = errors;
    }
}
