using Microsoft.AspNetCore.Mvc.Filters;
using Payment.Domain.Exceptions;
using Payment.Domain.Services.Rest;

namespace Payment.Api.Filters
{
    public class AuthorizationUser : IAsyncAuthorizationFilter
    {
        private readonly IUserRestService _userRest;

        public AuthorizationUser(IUserRestService userRest)
        {
            _userRest = userRest;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var user = await _userRest.GetUserInfos();
            } catch (RestException re)
            {
                throw new RestException(re.GetMessage(), System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}
