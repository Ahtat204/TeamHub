namespace TeamcollborationHub.server.Entities.Dto;

/// <summary>
/// 
/// </summary>
/// <param name="Username"></param>
/// <param name="Email"></param>
public record class UserRequestDto(
    string Email,
    string Password
);
public record class AuthenticationResponse(
    string email,
    string AccessToken,
    int ExpiryDate
);
