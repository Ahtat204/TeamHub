namespace TeamcollborationHub.server.Dto;

public sealed record RefreshTokenDto(string Token, string Id);

public sealed record RefreshAccessDto(string AccessToken, string RefreshToken);
