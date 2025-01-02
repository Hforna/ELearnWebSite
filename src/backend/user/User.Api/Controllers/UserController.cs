using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sqids;
using User.Api.DTOs;
using User.Api.Models;
using User.Api.Models.Repositories;
using User.Api.Services.Email;
using User.Api.Services.RequestValidators;
using User.Api.Services.Security.Cryptography;

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

        public UserController(IUnitOfWork uof, IBcryptCryptography cryptography, 
            EmailService emailService, IMapper mapper, 
            UserManager<UserModel> userManager, IConfiguration configuration)
        {
            _uof = uof;
            _cryptography = cryptography;
            _emailService = emailService;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserDto request)
        {
            var validator = new CreateUserValidator();
            var result = validator.Validate(request);

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
            }

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

            await _userManager.AddToRoleAsync(user, "customer");
            var tokenEmail = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var appUrl = _configuration.GetValue<string>("appUrl");

            await _emailService.SendEmail(request.Email, request.UserName, $"Welcome to our website {request.UserName}, " +
                "confirm your e-mail here:", $"{appUrl}/api/user/confirm-email?email={request.Email}&token={tokenEmail}");

            return Created(string.Empty, "We sent a e-mail confirmation to you");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string email, [FromQuery]string token)
        {
            var user = await _uof.userReadOnly.UserByEmail(email);

            if(user is null)
                throw new Exception();

            user.Active = true;

            var confirm = await _userManager.ConfirmEmailAsync(user, token);

            if (!confirm.Succeeded)
                throw new Exception();

            return Ok("E-mail confirmed");
        }
    }
}
