using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    private readonly IUserReadOnly _userRead;
    private readonly IConfiguration _configuration;


    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody]TokenDto request)
    {
        if(request is null)
            return BadRequest("Invalid Request");

        var validator = _tokenService.ValidateToken(request.Token!);

        if(validator.userGuid is null)
            return BadRequest("Invalid Access Token or Refresh Token");

        var user = await _userRead.UserByUid(validator.userGuid);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
            return BadRequest("Invalid access token or refresh token");

        var accessToken = _tokenService.GenerateToken((Guid)validator.userGuid, validator.claimsPrincipal.Claims.ToList());

        user.RefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiration = DateTime.UtcNow.AddHours(_configuration.GetValue<int>("jwt:refreshTokenExpirationHours"));

        await _userManager.UpdateAsync(user);

        var response = new ResponseLogin() { Token = user.RefreshToken, RefreshToken = user.RefreshToken };

        return Ok(response);
    }
} 