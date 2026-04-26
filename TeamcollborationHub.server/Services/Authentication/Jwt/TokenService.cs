using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Helpers;

namespace TeamcollborationHub.server.Services.Authentication.Jwt;

/// <summary>
/// Provides JWT access token and refresh token functionality.
/// </summary>
/// <remarks>
/// This service is responsible for:
/// - Generating signed JWT access tokens
/// - Generating cryptographically secure refresh tokens
/// - Persisting and validating refresh tokens
/// - Resolving users associated with refresh tokens
/// </remarks>
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string? _issuer;
    private readonly string? _audience;
    private readonly string? _key;
    private readonly string? _tokenValidityInMinutes; 
    private readonly TdbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class
    /// using application configuration and database context.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration containing JWT issuer, audience,
    /// signing key, and token duration settings.
    /// </param>
    /// <param name="context">
    /// Database context used for refresh token persistence and validation.
    /// </param>
    public TokenService(IConfiguration configuration, TdbContext context)
    {
        _configuration = configuration;
        _context = context;
        _key = _configuration["JwtConfig:Key"];
        _issuer = _configuration["JwtConfig:Issuer"];
        _audience = _configuration["JwtConfig:Audience"];
        _tokenValidityInMinutes=_configuration["JwtConfig:DurationInMinutes"];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class
    /// without dependencies.
    /// </summary>
    /// <remarks>
    /// This constructor is primarily intended for testing scenarios
    /// where token generation does not rely on persistence.
    /// </remarks>
    public TokenService()
    {
      
    }

    /// <summary>
    /// Generates a signed JWT access token for the specified user.
    /// </summary>
    /// <param name="user">
    /// The authenticated user for whom the token is generated.
    /// </param>
    /// <param name="exipryDuration">
    /// Outputs the remaining lifetime of the token in seconds.
    /// </param>
    /// <returns>
    /// A serialized JWT access token, or <c>null</c> if generation fails.
    /// </returns>
    /// <remarks>
    /// The token includes standard identity claims such as user ID,
    /// email, and name, and is signed using a symmetric HMAC SHA-256 key.
    /// </remarks>
    public string? GenerateTokenResponse(User user, out int exipryDuration)
    {
        var tokenExpiryDate = DateTime.UtcNow.AddMinutes(double.Parse(_tokenValidityInMinutes ?? "60"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            ]),
            Expires = tokenExpiryDate,
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key!)),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandles = new JwtSecurityTokenHandler();
        var securityToken = tokenHandles.CreateToken(tokenDescriptor);
        var accesstoken = tokenHandles.WriteToken(securityToken);
        exipryDuration = (int)tokenExpiryDate.Subtract(DateTime.UtcNow).TotalSeconds;
        return accesstoken;
    }

    /// <summary>
    /// Generates a cryptographically secure refresh token.
    /// </summary>
    /// <returns>
    /// A Base64-encoded random token string.
    /// </returns>
    /// <remarks>
    /// This token is intended to be stored securely and exchanged
    /// for new access tokens when the current access token expires.
    /// </remarks>
    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    /// <summary>
    /// Validates whether a refresh token exists in persistent storage.
    /// </summary>
    /// <param name="refreshToken">
    /// The refresh token string to validate.
    /// </param>
    /// <returns>
    /// The corresponding <see cref="RefreshToken"/> if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method does not validate token expiration or revocation
    /// beyond existence in the data store.
    /// </remarks>
    public async Task<RefreshToken?> ValidateRefreshToken(string refreshToken)
    {
        return await _context.RefreshTokens.Where(t => t.Token == refreshToken).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Persists a refresh token to the data store.
    /// </summary>
    /// <param name="refreshToken">
    /// The refresh token entity to save.
    /// </param>
    /// <returns>
    /// The stored refresh token string.
    /// </returns>
    /// <remarks>
    /// Successful persistence implies the refresh token can later
    /// be used for access token renewal.
    /// </remarks>
    public async Task<string?> SaveRefreshToken(RefreshToken refreshToken)
    {
        var result = _context.RefreshTokens.Add(refreshToken).Entity;
        await _context.SaveChangesAsync();
        return result.Token;
    }

    /// <summary>
    /// Retrieves the user associated with a given refresh token identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the refresh token.
    /// </param>
    /// <returns>
    /// The associated <see cref="User"/> if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method relies on a navigation relationship between
    /// refresh tokens and users.
    /// </remarks>
    public async Task<User?> GetUserByRefreshToken(Guid id) => await _context.RefreshTokens.Where(re => re.Id == id)
        .Select(u => u.User).FirstOrDefaultAsync();
}