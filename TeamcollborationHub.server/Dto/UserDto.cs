namespace TeamcollborationHub.server.Dto;

public record RefreshTokenDto(string Token, string Id);

public record RefreshAccessDto(string AccessToken,string RefreshToken);
