using Microsoft.AspNetCore.Mvc;
using Progress.Api.Filters;

namespace Progress.Api.Attributes
{
    public class UserAuthenticated : TypeFilterAttribute
    {
        public UserAuthenticated() : base(typeof(AuthorizationUser))
        {
        }
    }
}
