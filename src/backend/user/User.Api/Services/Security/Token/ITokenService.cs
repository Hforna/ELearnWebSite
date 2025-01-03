using System.Security.Claims;
using User.Api.Models;

namespace User.Api.Services.Security.Token
{
    public interface ITokenService
    {
        public string GenerateToken(Guid uid, List<Claim> claims);
        public (ClaimsPrincipal claimsPrincipal, Guid? userGuid) ValidateToken(string token);
        public string GenerateRefreshToken();
        public Task<UserModel?> UserByToken(string token);
    }
}
