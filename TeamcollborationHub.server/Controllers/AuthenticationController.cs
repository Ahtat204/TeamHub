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

    [HttpPost("/login")]
    public async Task<ActionResult<AuthenticationResponseDto>> Login([FromBody] UserRequestDto? userCridentials)
    {
        if(userCridentials is null)  return BadRequest("Invalid user data"); 
        var result = await _authenticationService.AuthenticateUser(userCridentials);
        if(result is null) return NotFound("Invalid user data"); 
        var token=_jwtService.GenerateTokenResponse(result,out var date);
        return Ok(new AuthenticationResponseDto(result.Email, token, date));
        
    }
    [HttpPost("/signup")]
    public async Task<ActionResult<RegisterUserDto>> SignUp([FromBody] CreateUserDto? user)
    {
        if(user is null) return BadRequest("Invalid user data");
        var result = await _authenticationService.CreateUser(user);
        if(result is null) return BadRequest("Invalid user data");
        return Ok(new RegisterUserDto(result.Email, result.Name));
    }
}
