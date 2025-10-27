using Course.Domain.Services.Rest;
using Course.Exception;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Course.Api.Filters
{
    public class AuthorizationUser : IAsyncAuthorizationFilter
    {
        private IUserService _userService;

        public AuthorizationUser(IUserService userService) => _userService = userService;

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var isUserlogged = await _userService.IsUserLogged();

            if (!isUserlogged)
                throw new NotAuthenticatedException(ResourceExceptMessages.USER_NOT_LOGGED);
        }
    }
}
