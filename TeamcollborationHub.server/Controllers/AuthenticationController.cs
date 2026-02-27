using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Caching;

namespace TeamcollborationHub.server.Controllers;

/// <summary>
/// Exposes authentication-related HTTP endpoints.
/// </summary>
/// <remarks>
/// This controller handles:
/// - User login
/// - User registration
/// - Access token refresh using refresh tokens
/// 
/// It orchestrates authentication, JWT generation,
/// and refresh token persistence/caching.
/// </remarks>

[Route("api")]
[ApiController]
public class AuthenticationController(
    IAuthenticationService authenticationService,
    IJwtService jwtService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService =
        authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

    /// <summary>
    /// Authenticates a user and issues access and refresh tokens.
    /// </summary>
    /// <param name="userCridentials">
    /// The login credentials containing email and password.
    /// </param>
    /// <returns>
    /// A <see cref="LoginResponseDto"/> containing the access token,
    /// refresh token, and token expiration metadata.
    /// </returns>
    /// <response code="200">
    /// Authentication succeeded and tokens were generated.
    /// </response>
    /// <response code="400">
    /// The request body is null or malformed.
    /// </response>
    /// <response code="404">
    /// Authentication failed due to invalid credentials.
    /// </response>
    /// <remarks>
    /// On successful authentication:
    /// - A JWT access token is generated
    /// - A refresh token is generated and persisted
    /// - The refresh token is associated with the authenticated user
    /// </remarks>
    [HttpPost("/login")]
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

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="user">
    /// The data required to create a new user.
    /// </param>
    /// <returns>
    /// A <see cref="RegisterUserDto"/> representing the newly created user.
    /// </returns>
    /// <response code="200">
    /// The user was successfully created.
    /// </response>
    /// <response code="400">
    /// The provided user data is invalid or incomplete.
    /// </response>
    /// <remarks>
    /// This endpoint delegates validation and persistence logic
    /// to the authentication service.
    /// </remarks>
    [HttpPost("/signup")]
    public async Task<ActionResult<RegisterUserDto>> SignUp([FromBody] CreateUserDto? user)
    {
        if (user is null) return BadRequest("Invalid user data");
        var result = await _authenticationService.CreateUser(user);
        if (result is null) return BadRequest("Invalid user data");
        return Ok(new RegisterUserDto(result.Email, result.Name));
    }

    /// <summary>
    /// Issues a new access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshToken">
    /// The refresh token payload containing the token value
    /// and its identifier.
    /// </param>
    /// <returns>
    /// A <see cref="RefreshAccessDto"/> containing a new access token
    /// and a newly issued refresh token.
    /// </returns>
    /// <response code="200">
    /// A new access token was successfully generated.
    /// </response>
    /// <response code="400">
    /// No refresh token was provided in the request.
    /// </response>
    /// <response code="404">
    /// The refresh token does not exist or is invalid.
    /// </response>
    /// <remarks>
    /// This endpoint:
    /// - Validates the provided refresh token
    /// - Generates a new access token
    /// - Rotates the refresh token by issuing a new one
    /// 
    /// Expired or missing tokens result in a failed request.
    /// </remarks>
    [HttpPost("/refresh")]
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