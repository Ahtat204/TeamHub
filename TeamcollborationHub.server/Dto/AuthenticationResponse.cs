namespace TeamcollborationHub.server.Dto;

public sealed record AuthenticationResponse(
    string email,
    string AccessToken,
    int ExpiryDate
);