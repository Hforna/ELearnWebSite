using Microsoft.AspNetCore.Mvc;
using Payment.Api.Filters;

namespace Payment.Api.Attributes
{
    public class UserAuthenticatedAttribute : TypeFilterAttribute
    {
        public UserAuthenticatedAttribute() : base(typeof(AuthorizationUser))
        {
        }
    }
}
