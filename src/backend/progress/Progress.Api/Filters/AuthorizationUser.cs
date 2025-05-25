using Microsoft.AspNetCore.Mvc.Filters;
using Progress.Domain.Exceptions;
using Progress.Domain.Rest;

namespace Progress.Api.Filters
{
    public class AuthorizationUser : IAsyncAuthorizationFilter
    {
        private IUserRestService _userService;

        public AuthorizationUser(IUserRestService userService) => _userService = userService;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var isUserlogged = await _userService.IsUserLogged();

            if (!isUserlogged)
                throw new RestException(ResourceExceptMessages.USER_NOT_AUTHENTICATED, System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
