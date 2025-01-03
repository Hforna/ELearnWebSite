using Microsoft.AspNetCore.Mvc;
using User.Api.Filters;

namespace User.Api.Attributes
{
    public class AuthenticationUserAttribute : TypeFilterAttribute
    {
        public AuthenticationUserAttribute() : base(typeof(AuthorizationUser))
        {
        }
    }
}
