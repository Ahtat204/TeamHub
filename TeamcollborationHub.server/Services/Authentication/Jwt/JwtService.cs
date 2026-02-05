using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;


namespace TeamcollborationHub.server.Services.Authentication.Jwt;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly TdbContext context;

    public JwtService()
    {
    }

    public JwtService(IConfiguration configuration, TdbContext context)
    {
        _configuration = configuration;
        context = context;
    }

    public string? GenerateTokenResponse(User user, out int exipryDuration)
    {
        var issuer = _configuration["JwtConfig:Issuer"];
        var audience = _configuration["JwtConfig:Audience"];
        var key = _configuration["JwtConfig:Key"];
        var tokenValidityInMinutes = _configuration["JwtConfig:DurationInMinutes"];
        var tokenExpiryDate = DateTime.UtcNow.AddMinutes(double.Parse(tokenValidityInMinutes ?? "60"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            ]),
            Expires = tokenExpiryDate,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandles = new JwtSecurityTokenHandler();
        var securityToken = tokenHandles.CreateToken(tokenDescriptor);
        var accesstoken = tokenHandles.WriteToken(securityToken);
        exipryDuration = (int)tokenExpiryDate.Subtract(DateTime.UtcNow).TotalSeconds;
        return accesstoken;
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public async Task<RefreshToken?> ValidateRefreshToken(string refreshToken)
    {
        return await context.RefreshTokens.Where(t => t.Token == refreshToken).FirstOrDefaultAsync();
    }

    public async Task<string?> SaveRefreshToken(RefreshToken refreshToken)
    {
        var result = context.RefreshTokens.Add(refreshToken).Entity;
        await context.SaveChangesAsync();
        return result.Token;
    }

    public async Task<User?> GetUserByRefreshToken(Guid id) => await context.RefreshTokens.Where(re => re.Id == id)
        .Select(u => u.User).FirstOrDefaultAsync();
}