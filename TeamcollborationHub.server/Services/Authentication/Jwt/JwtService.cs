using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;

namespace TeamcollborationHub.server.Services.Authentication.Jwt;

public class JwtService: IJwtService
{
    private readonly TDBContext _tDBContext;
    private readonly IConfiguration configuration;
    public JwtService(TDBContext tDBContext, IConfiguration _configuration)
    {
        _tDBContext = tDBContext;
        configuration = _configuration;
    }

    public  string? GenerateTokenResponse(User user,out int expiryDate)
    {

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key = configuration["Jwt:Key"];
        var tokenValidityInMinutes = configuration["Jwt:DurationInMinutes"];
        var tokenExpiryDate = DateTime.UtcNow.AddMinutes(double.Parse(tokenValidityInMinutes ?? "60"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            }),
            Expires = tokenExpiryDate,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key!)), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandles = new JwtSecurityTokenHandler();
        var securityToken = tokenHandles.CreateToken(tokenDescriptor);
        var accesstoken = tokenHandles.WriteToken(securityToken);
        expiryDate = (int)tokenExpiryDate.Subtract(DateTime.UtcNow).TotalSeconds;
        return accesstoken;
    }
}
