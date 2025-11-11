namespace TeamcollborationHub.server.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpGet("/login")]
    public async Task<ActionResult<User>> Login([FromBody] User user)
    {
        if(user is null) { return BadRequest("Invalid user data"); }
        return Ok();///TODO: implement login logic
    }


    [HttpPost("/signup")]
    public async Task<ActionResult<User>> SignUp([FromBody] User user)
    {
        return Ok();///TODO: implement signup logic
    }
}
