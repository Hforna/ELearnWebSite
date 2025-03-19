using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Token
{
    public interface ITokenReceptor
    {
        public string? GetToken();
    }
}
