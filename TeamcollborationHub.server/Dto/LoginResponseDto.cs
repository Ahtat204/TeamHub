namespace TeamcollborationHub.server.Dto;

public record LoginResponseDto(
    string email,
    string? AccessToken,
    int ExpiryDate,
    RefreshTokenDto? RefreshToken=null
);
