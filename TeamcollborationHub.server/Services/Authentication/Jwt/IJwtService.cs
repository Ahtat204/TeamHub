using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;

namespace TeamcollborationHub.server.Services.Authentication.Jwt;

public interface IJwtService
{
    public string? GenerateTokenResponse(User userRequest, out int expiryDate);
    public string GenerateRefreshToken();
    public Task<RefreshToken?> ValidateRefreshToken(string refreshToken);
    public  Task<string?> SaveRefreshToken(RefreshToken refreshToken);
    public Task<User?> GetUserByRefreshToken(Guid id); 
}