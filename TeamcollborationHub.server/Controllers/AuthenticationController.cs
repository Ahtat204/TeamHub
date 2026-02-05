using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Caching;
using TeamcollborationHub.server.Helpers;

namespace TeamcollborationHub.server.Controllers;

[Route("api")]
[ApiController]
public class AuthenticationController(
    IAuthenticationService authenticationService,
    IJwtService jwtService,
    ICachingService<RefreshToken, string> refreshTokenCachingService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService =
        authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto? userCridentials)
    {
        if (userCridentials is null) return BadRequest("Invalid user data");
        var result = await _authenticationService.AuthenticateUser(userCridentials);
        if (result is null) return NotFound("Invalid user data");
        var token = jwtService.GenerateTokenResponse(result, out var date);
        var generateRefreshToken = jwtService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            Token = generateRefreshToken,
            Expires = DateTime.UtcNow.AddDays(10),
            UserId = result.Id,
            Id = Guid.NewGuid(),
        };
       // refreshTokenCachingService.SetProjectInCache(refreshToken.Id.ToString(), refreshToken);
        await jwtService.SaveRefreshToken(refreshToken);
        return Ok(new LoginResponseDto(result.Email, token, date, new RefreshTokenDto(generateRefreshToken,refreshToken.Id.ToString())));
    }

    [HttpPost("signup")]
    public async Task<ActionResult<RegisterUserDto>> SignUp([FromBody] CreateUserDto? user)
    {
        if (user is null) return BadRequest("Invalid user data");
        var result = await _authenticationService.CreateUser(user);
        if (result is null) return BadRequest("Invalid user data");
        return Ok(new RegisterUserDto(result.Email, result.Name));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshAccessDto>> Refresh([FromBody] RefreshTokenDto? refreshToken)
    {
        if (refreshToken?.Token is null) return BadRequest("no refresh token found");
        var found =await jwtService.ValidateRefreshToken(refreshToken.Token);
        
        if (found is  null)
        {
            return NotFound("Invalid refresh token");
        }
        
        RefreshToken newRefreshToken = new()
        {
            Token = jwtService.GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(10),
            UserId = found.UserId,
            Id = Guid.NewGuid(),
        };
        var user=await jwtService.GetUserByRefreshToken(found.Id);
        var accessToken=jwtService.GenerateTokenResponse(user, out var date);
        var result=await jwtService.SaveRefreshToken(newRefreshToken);
        return Ok(new RefreshAccessDto(accessToken,newRefreshToken.Token));

    }
}