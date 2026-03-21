namespace TeamcollborationHub.server.Dto;

public sealed record LoginResponseDto(
    string email,
    string? AccessToken,
    int ExpiryDate,
    RefreshTokenDto? RefreshToken = null
);
