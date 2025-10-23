using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class UnauthorizedException : BaseException
    {
        public override List<string> Errors { get; set; } = [];

        public UnauthorizedException(string error) => Errors.Add(error);
        public UnauthorizedException(List<string> errors) => Errors = errors;
    }
}
