using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User.Api.DTOs;
using User.Api.Models;
using User.Api.Models.Repositories;
using User.Api.Services.Email;
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
        private readonly EmailService _emailService;

        public LoginController(ITokenService tokenService, IUserReadOnly userRead, 
        IBcryptCryptography bcrypt, UserManager<UserModel> userManager, 
        IConfiguration configuration, EmailService emailService)
        {
            _tokenService = tokenService;
            _userRead = userRead;
            _bcrypt = bcrypt;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginByApp([FromBody]LoginDto request)
        {
            var user = await _userRead.UserByEmail(request.Email);

            if (user is null || !_bcrypt.IsKeyValid(request.Password, user.PasswordHash!))
                throw new Exception("E-mail or password invalid");

            if(user.TwoFactorEnabled)
            {
                var twofaCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                await _emailService.SendEmail(user.Email, user.UserName, "Two factor verification", $"Your code is: {twofaCode}");


                return Ok(new { required2fa = true, Message = "We sent a e-mail for two factor code" } );
            }

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

            return Ok(new ResponseLogin() { AccessToken = token, RefreshToken = refreshToken});
        }

        [HttpGet("2fa/verify")]
        public async Task<IActionResult> VerfiyTwoFactorAuthentication([FromQuery]string code, [FromQuery]string email)
        {
            var user = await _userRead.UserByEmail(email);

            if (user is null)
                return BadRequest("user e-mail is wrong");

            var isValidCode = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);

            if (!isValidCode)
                return BadRequest("Code is wrong");

            var claims = new List<Claim>();

            var roles = await _userManager.GetRolesAsync(user);

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var accessToken = _tokenService.GenerateToken(user.UserIdentifier, claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("jwt:refreshTokenExpirationHours"));

            await _userManager.UpdateAsync(user);

            var response = new ResponseLogin() { AccessToken = accessToken, RefreshToken = refreshToken};

            return Ok(response);
        }
    }
}
