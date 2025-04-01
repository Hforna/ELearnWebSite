using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using User.Api.Attributes;
using User.Api.DTOs;
using User.Api.Models;
using User.Api.Models.Repositories;
using User.Api.Services.Security.Token;

namespace User.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<UserModel> _userManager;
    private readonly IUnitOfWork _uof;
    private readonly IConfiguration _configuration;

    public TokenController(ITokenService tokenService, UserManager<UserModel> userManager, IUnitOfWork uof, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _uof = uof;
        _configuration = configuration;
    }

    [AuthenticationUser]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody]TokenDto request)
    {
        if(request is null)
            return BadRequest("Invalid Request");

        var validator = _tokenService.ValidateToken(request.Token!);

        if(validator.userGuid is null)
            return BadRequest("Invalid Access Token or Refresh Token");

        var user = await _uof.userReadOnly.UserByUid(validator.userGuid);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
            return BadRequest("Invalid access token or refresh token");

        var accessToken = _tokenService.GenerateToken((Guid)validator.userGuid, validator.claimsPrincipal.Claims.ToList());

        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("jwt:refreshTokenExpirationHours"));

        await _userManager.UpdateAsync(user);

        var response = new ResponseLogin() { AccessToken = accessToken, RefreshToken = user.RefreshToken };

        return Ok(response);
    }
} 