using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Services.Authentication.Jwt;

namespace TeamcollborationHub.server.Controllers;
[Route("api")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IJwtService _jwtService;

    public AuthenticationController(IAuthenticationService authenticationService, IJwtService jwtService)
    {
        _jwtService = jwtService;
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto? userCridentials)
    {
        if(userCridentials is null)  return BadRequest("Invalid user data"); 
        var result = await _authenticationService.AuthenticateUser(userCridentials);
        if(result is null) return NotFound("Invalid user data"); 
        var token=_jwtService.GenerateTokenResponse(result,out var date);
        var refreshToken=_jwtService.GenerateRefreshToken();
        var refreshToken1 = new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(10),
            UserId = result.Id,
            Id = Guid.NewGuid(),
        };
        var save=await _authenticationService.SaveRefreshToken(refreshToken1);
        return Ok(new LoginResponseDto(result.Email, token, date,refreshToken));
        
    }
    [HttpPost("signup")]
    public async Task<ActionResult<RegisterUserDto>> SignUp([FromBody] CreateUserDto? user)
    {
        if(user is null) return BadRequest("Invalid user data");
        var result = await _authenticationService.CreateUser(user);
        if(result is null) return BadRequest("Invalid user data");
        return Ok(new RegisterUserDto(result.Email, result.Name));
    }

    /*[HttpPost("refresh")]
    public async Task<ActionResult<RefreshAccessDto>> Refresh([FromBody] string? refreshToken)
    {
        if(refreshToken is null) return BadRequest("Invalid refresh token");
    }*/
}
