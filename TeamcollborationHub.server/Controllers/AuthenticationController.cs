using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;



using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    [HttpGet("/login")]
    public async Task<ActionResult<User>> Login([FromBody] UserRequestDto userCridentials)
    {
        if(userCridentials is null)  return BadRequest("Invalid user data"); 
        var response = await _authenticationService.AuthenticateUser(userCridentials);
        if(response is null) return BadRequest("Invalid user data"); 

        return Ok(response);

    }


    [HttpPost("/signup")]
    public async Task<ActionResult<User>> SignUp([FromBody] User user)
    {
        return Ok();///TODO: implement signup logic
    }
}
