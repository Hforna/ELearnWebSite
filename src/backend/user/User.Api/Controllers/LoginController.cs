using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User.Api.DTOs;
using User.Api.Models;
using User.Api.Models.Repositories;
using User.Api.Services.Security.Cryptography;
using User.Api.Services.Security.Token;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserReadOnly _userRead;
        private readonly IBcryptCryptography _bcrypt;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;

        public LoginController(ITokenService tokenService, IUserReadOnly userRead, 
        IBcryptCryptography bcrypt, UserManager<UserModel> userManager, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userRead = userRead;
            _bcrypt = bcrypt;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> LoginByApp([FromBody]LoginDto request)
        {
            var user = await _userRead.UserByEmail(request.Email);

            if (user is null || _bcrypt.IsKeyValid(request.Password, user.PasswordHash!))
                throw new Exception("E-mail or password invalid");

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>();

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
  
            var refreshToken = _tokenService.GenerateRefreshToken(); 
            var token = _tokenService.GenerateToken(user.UserIdentifier, claims);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("jwt:refreshTokenExpirationHours"));

            await _userManager.UpdateAsync(user);

            return Ok(new ResponseLogin() { Token = token, RefreshToken = refreshToken});
        }
    }
}
