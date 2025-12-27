using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Services.Authentication.Jwt;

namespace TeamcollborationHub.server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IAuthenticationService authenticationService, IJwtService jwtservice) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

    [HttpPost("/login")]
    public async Task<ActionResult<AuthenticationResponseDto>> Login([FromBody] UserRequestDto? userCridentials)
    {
        if(userCridentials is null)  return BadRequest("Invalid user data"); 
        var result = await _authenticationService.AuthenticateUser(userCridentials);
        if(result is null) return BadRequest("Invalid user data"); 
        var token=jwtservice.GenerateTokenResponse(result,out var date);
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
