namespace TeamcollborationHub.server.Dto;

public record AuthenticationResponse(
    string email,
    string AccessToken,
    int ExpiryDate
);