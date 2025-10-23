using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Exception
{
    public class NotFoundException : BaseException
    {
        public override List<string> Errors { get; set; } = [];

        public NotFoundException(string error) => Errors.Add(error);
        public NotFoundException(List<string> errors) => Errors = errors;
    }
}
