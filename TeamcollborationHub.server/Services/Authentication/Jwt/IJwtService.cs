using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Authentication.Jwt;

public interface IJwtService
{
    public string? GenerateTokenResponse(User userRequest, out int expiryDate);
}
