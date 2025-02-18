﻿using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sqids;
using User.Api.Attributes;
using User.Api.DTOs;
using User.Api.Excpetions;
using User.Api.Filters.Token;
using User.Api.Models;
using User.Api.Models.Repositories;
using User.Api.Responses;
using User.Api.Services.Email;
using User.Api.Services.RequestValidators;
using User.Api.Services.Security.Cryptography;
using User.Api.Services.Security.Token;

namespace User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IBcryptCryptography _cryptography;
        private readonly EmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UserManager<UserModel> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ITokenReceptor _tokenReceptor;

        public UserController(IUnitOfWork uof, IBcryptCryptography cryptography, 
            EmailService emailService, IMapper mapper, 
            UserManager<UserModel> userManager, IConfiguration configuration, 
            ITokenService tokenService, ITokenReceptor tokenReceptor)
        {
            _uof = uof;
            _cryptography = cryptography;
            _emailService = emailService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _tokenService = tokenService;
            _tokenReceptor = tokenReceptor;
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserDto request)
        {
            ValidateGenericRequest<CreateUserValidator>(request);

            if (await _uof.userReadOnly.EmailExists(request.Email))
                throw new Exception();

            var user = _mapper.Map<UserModel>(request);

            user.SecurityStamp = Guid.NewGuid().ToString();
            user.PasswordHash = _cryptography.GenerateCryptography(request.Password);
            user.EmailConfirmed = false;
            user.Active = false;
            user.UserIdentifier = Guid.NewGuid();

            await _uof.userWriteOnly.CreateUser(user);
            await _uof.Commit();

            var userRole = request.isTeacher ? "teacher" : "customer";

            await _userManager.AddToRoleAsync(user, userRole);
            var tokenEmail = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var appUrl = _configuration.GetValue<string>("appUrl");

            await _emailService.SendEmail(request.Email, request.UserName, $"Welcome to our website {request.UserName}, " +
                "confirm your e-mail here:", $"{appUrl}/api/user/confirm-email?email={request.Email}&token={tokenEmail}");

            return Created(string.Empty, "We sent a e-mail confirmation to you");
        }

        [AuthenticationUser]
        [HttpGet("get-user-roles/{uid}")]
        public async Task<IActionResult> GetUserRoles([FromRoute]string uid)
        {
            var user = await _uof.userReadOnly.UserByUid(Guid.Parse(uid));

            if (user is null)
                return BadRequest("User doesn't exists");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(roles);
        }

        [Authorize(Policy = "StaffOnly")]
        [HttpPost("add-user-role")]
        public async Task<IActionResult> AddUserToRole([FromBody]AddUserToRole request)
        {
            var user = await _uof.userReadOnly.UserByEmail(request.email);

            if (user is null)
                return BadRequest("Email user doesn't exists");

            await _userManager.AddToRoleAsync(user, request.role);

            return NoContent();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string email, [FromQuery]string token)
        {
            var user = await _uof.userReadOnly.UserByEmail(email);

            if(user is null)
                throw new Exception("User doesn't exists");

            var confirm = await _userManager.ConfirmEmailAsync(user, token);

            if (!confirm.Succeeded)
                throw new Exception("Wrong token for email confirmation");

            user.Active = true;
            user.EmailConfirmed = true;

            await _uof.Commit();

            return Ok("E-mail confirmed");
        }

        [AuthenticationUser]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody]UpdatePasswordDto request)
        {
            var user = await _tokenService.UserByToken(_tokenReceptor.GetToken());

            if (!_cryptography.IsKeyValid(request.OldPassword, user.PasswordHash))
                throw new BadHttpRequestException("Invalid password");

            if (request.NewPassword.Length < 8)
                throw new BadHttpRequestException("New password length must be 8 digits or more");

            user.PasswordHash = _cryptography.GenerateCryptography(request.NewPassword);
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [HttpGet("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromQuery]string email)
        {
            var user = await _uof.userReadOnly.UserByEmail(email);

            if (user is null)
                throw new BadHttpRequestException("E-mail doesn't exists");

            var appUrl = _configuration.GetValue<string>("appUrl");

            var emailtoken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendEmail(email, user.UserName, "Reset password e-mail", 
                $"Reset you password here: {appUrl}/api/user/reset-password?email={email}&token={emailtoken}");

            return Ok("We sent a reset password e-mail to you");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetUserPassword([FromBody]ResetPasswordDto request, [FromQuery]string email, [FromQuery]string token)
        {
            ValidateGenericRequest<ResetPasswordValidator>(request);

            var user = await _uof.userReadOnly.UserByEmail(email);

            if (user is null)
                throw new BadHttpRequestException("E-mail doesn't exists");

            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

            if (!isValidToken)
                throw new BadHttpRequestException("Invalid reset password token");

            user.PasswordHash = _cryptography.GenerateCryptography(request.Password);
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [AuthenticationUser]
        [HttpGet("user-infos")]
        public async Task<IActionResult> UserInfos()
        {
            var user = await _tokenService.UserByToken(_tokenReceptor.GetToken());

            var response = _mapper.Map<UserResponse>(user);

            return Ok(response);
        }

        [AuthenticationUser]
        [HttpGet("2fa/request")]
        public async Task<IActionResult> RequestTwoFactorAuthentication()
        {
            var user = await _tokenService.UserByToken(_tokenReceptor.GetToken());

            if (user.TwoFactorEnabled)
                throw new RequestException("Two factor already is enabled in this account");

            var twofaCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

            await _emailService.SendEmail(user.Email!, user.UserName!, "Enable two factor code", $"Your enable code is: {twofaCode}");

            return NoContent();
        }

        [AuthenticationUser]
        [HttpGet("2fa/enable")]
        public async Task<IActionResult> EnableTwoFactorAuthentication([FromQuery]string code)
        {
            var user = await _tokenService.UserByToken(_tokenReceptor.GetToken());

            var codeVerify = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);

            if (!codeVerify)
                return BadRequest("Code is wrong");

            user.TwoFactorEnabled = true;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [AuthenticationUser]
        [HttpGet("2fa/status")]
        public async Task<IActionResult> GetTwoFactorAuthenticationStatus()
        {
            var user = await _tokenService.UserByToken(_tokenReceptor.GetToken());
            return Ok(new { isEnabled = user.TwoFactorEnabled });
        }

        [AuthenticationUser]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {

            var user = await _tokenService.UserByToken(_tokenReceptor.GetToken());

            user.Active = false;
            user.TimeDisabled = DateTime.UtcNow.AddDays(30);

            var token = await _userManager.GenerateUserTokenAsync(user, "Email", "ReactivateAccount");

            await _userManager.UpdateAsync(user);

            var appUrl = _configuration.GetValue<string>("appUrl");

            await _emailService.SendEmail(user.Email, user.UserName, "You account is disabled", 
                $"you have 30 days for active your account again otherwise it will be deleted, " +
                $"click here to reactivate your account: {appUrl}/user/reactivate-account?email={user.Email}&token={token}");

            return NoContent();
        }

        [HttpGet("reactivate-account")]
        public async Task<IActionResult> ReactivateAccount([FromQuery]string email, [FromQuery]string token)
        {
            var user = await _uof.userReadOnly.UserByEmail(email);

            if (user is null || await _userManager.VerifyUserTokenAsync(user, "Email", "ReactivateAccount", token))
                return BadRequest("Invalid request");

            user.Active = true;
            user.TimeDisabled = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [AuthenticationUser]
        [HttpGet("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var user = await _tokenService.UserByToken(_tokenReceptor.GetToken());

            user.RefreshToken = null;
            user.RefreshTokenExpiration = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        private static void ValidateGenericRequest<Validator>(object request) where Validator : IValidator
        {
            var validator = Activator.CreateInstance<Validator>();
            var result = validator.Validate(request);

            if(result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new RequestException(errorMessages);
            }
        }
    }
}
