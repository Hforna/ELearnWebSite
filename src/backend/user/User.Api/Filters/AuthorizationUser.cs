using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using User.Api.Filters.Token;
using User.Api.Models.Repositories;
using User.Api.Services.Security.Token;

namespace User.Api.Filters
{
    public class AuthorizationUser : IAsyncAuthorizationFilter
    {
        private readonly ITokenReceptor _tokenReceptor;
        private readonly ITokenService _tokenService;
        private readonly IUserReadOnly _userRead;

        public AuthorizationUser(ITokenReceptor tokenReceptor, ITokenService tokenService, IUserReadOnly userRead)
        {
            _tokenReceptor = tokenReceptor;
            _tokenService = tokenService;
            _userRead = userRead;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var validator = _tokenService.ValidateToken(_tokenReceptor.GetToken());

                var user = await _userRead.UserByUid(validator.userGuid);

                if (user is null)
                    throw new BadHttpRequestException("invalid token");
            }
            catch (SecurityTokenExpiredException ex)
            {
                context.Result = new UnauthorizedObjectResult(new BadRequestObjectResult(ex.Message));                
            }
        }
    }
}
