using Microsoft.AspNetCore.Http;
using Progress.Domain.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Infrastructure.Rest
{
    public class UserRestException
    {
        private readonly IHttpContextFactory httpContext;
        private readonly ITokenReceptor tokenReceptor;

        public UserRestException(IHttpContextFactory httpContext, ITokenReceptor tokenReceptor)
        {
            this.httpContext = httpContext;
            this.tokenReceptor = tokenReceptor;
        }

        public 
    }
}
