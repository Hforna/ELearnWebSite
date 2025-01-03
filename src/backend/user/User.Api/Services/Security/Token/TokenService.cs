using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using User.Api.Models;
using User.Api.Models.Repositories;

namespace User.Api.Services.Security.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserReadOnly _userRead;
        private readonly string _signKey;
        private readonly int _accessExpiration;

        public TokenService(IConfiguration configuration, IUserReadOnly userRead)
        {
            _configuration = configuration;
            _userRead = userRead;
            _signKey = _configuration.GetValue<string>("jwt:signKey")!;
            _accessExpiration = _configuration.GetValue<int>("jwt:accessExpiration");
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string GenerateToken(Guid uid, List<Claim> claims)
        {
            var signingCredentials = new SigningCredentials(GetSignKey(), SecurityAlgorithms.HmacSha256Signature);

            claims.Add(new Claim(ClaimTypes.Sid, uid.ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.AddMinutes(_accessExpiration),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials                
            };

            var tokenManipulation = new JwtSecurityTokenHandler();

            var createToken = tokenManipulation.CreateToken(tokenDescriptor);

            return tokenManipulation.WriteToken(createToken);
        }

        public async Task<UserModel?> UserByToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var read = tokenHandler.ReadJwtToken(token);

            var uid = read.Claims.FirstOrDefault(d => d.Type == ClaimTypes.Sid)?.Value;

            return await _userRead.UserByUid(Guid.Parse(uid!));
        }

        public (ClaimsPrincipal claimsPrincipal, Guid? userGuid) ValidateToken(string token)
        {
            var validatorParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSignKey(),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, validatorParameters, out var securityToken);

            if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, 
            StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            var uid = Guid.Parse(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);

            return (claimsPrincipal: principal, userGuid: uid);
        }

        private SymmetricSecurityKey GetSignKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signKey));
        }
    }
}
