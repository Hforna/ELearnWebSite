using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Twilio.Rest.Verify.V2.Service;
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
        private readonly IUnitOfWork _uof;
        private readonly IBcryptCryptography _bcrypt;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public LoginController(ITokenService tokenService, IUnitOfWork uof, 
        IBcryptCryptography bcrypt, UserManager<UserModel> userManager, 
        IConfiguration configuration, EmailService emailService)
        {
            _tokenService = tokenService;
            _uof = uof;
            _bcrypt = bcrypt;
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> LoginByApp([FromBody]LoginDto request)
        {
            var user = await _uof.userReadOnly.UserByEmail(request.Email);

            if (user is null || !_bcrypt.IsKeyValid(request.Password, user.PasswordHash!) || !user.Active)
                throw new Exception("E-mail or password invalid");

            if(user.TwoFactorEnabled)
            {
                var twofaCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                await _emailService.SendEmail(user.Email, user.UserName, "Two factor verification", $"Your code is: {twofaCode}");


                return Ok(new { required2fa = true, 
                    Methods = $"[email, phone]", Message = "We sent a e-mail for two factor code" } );
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

        [HttpPost("2fa/send-code")]
        public async Task<IActionResult> VerifyTwoFactorAuthenticationCode([FromBody]TwoFactorSendDto dto)
        {
            if (dto.method != "phone" && dto.method != "email")
                return BadRequest("Two factor method not allowed");

            var user = await _uof.userReadOnly.UserByEmail(dto.email);

            if (user is null)
                return NotFound("User doesn't exists");

            if (!user.TwoFactorEnabled)
                return Unauthorized("User 2fa isn't enabled");

            var twofaMethod = dto.method;

            if ((twofaMethod == "email" && !user.TwoFactorEmailEnabled) 
                || (twofaMethod == "phone" && !user.TwoFactorPhoneEnabled))
                return Unauthorized("2fa method not authorized for this user");

            if(twofaMethod == "email")
            {
                var twofaCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                await _emailService.SendEmail(user.Email, user.UserName, "Two factor verification", 
                    $"Your code is: {twofaCode}");
            } else if(twofaMethod == "phone")
            {
                var serviceSid = _configuration.GetValue<string>("services:twilio:serviceSid");

                var verification = await VerificationResource.CreateAsync(to: user.PhoneNumber,
                    channel: "sms", pathServiceSid: serviceSid);

                if (verification.Status != "pending" && verification.Status != "approved")
                    return BadRequest("message not sent");
            }

            return Ok();
        }

        [HttpGet("2fa/phone/verify")]
        public async Task<IActionResult> VerifyTwoFactorAuthenticationByPhone([FromQuery]string code, [FromQuery]string email)
        {
            var user = await _uof.userReadOnly.UserByEmail(email);

            if(user is null)
                return BadRequest("user e-mail is wrong");

            var serviceSid = _configuration.GetValue<string>("services:twilio:serviceSid");

            var verify = await VerificationCheckResource.CreateAsync(to: user.PhoneNumber, code: code,
                pathServiceSid: serviceSid);

            if (!(bool)verify.Valid!)
                throw new Exception("code cannot be verified, try again later");

            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>();

            foreach(var role in userRoles)
            {
                var claim = new Claim(ClaimTypes.Role, role);
                claims.Add(claim);
            }

            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("jwt:refreshTokenExpirationHours"));

            var response =  new ResponseLogin()
            {
                AccessToken = _tokenService.GenerateToken(user.UserIdentifier, claims),
                RefreshToken = refreshToken
            };

            return Ok(response);
        }

        [HttpGet("2fa/email/verify")]
        public async Task<IActionResult> VerfiyTwoFactorAuthenticationByEmail([FromQuery]string code, [FromQuery]string email)
        {
            var user = await _uof.userReadOnly.UserByEmail(email);

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
