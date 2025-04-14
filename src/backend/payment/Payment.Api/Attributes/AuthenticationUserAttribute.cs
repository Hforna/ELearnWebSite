using Microsoft.AspNetCore.Mvc;
using Payment.Api.Filters;

namespace Payment.Api.Attributes
{
    public class AuthenticationUserAttribute : TypeFilterAttribute
    {
        public AuthenticationUserAttribute() : base(typeof(AuthorizationUser))
        {
        }
    }
}
