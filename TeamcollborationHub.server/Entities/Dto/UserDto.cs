using System.ComponentModel.DataAnnotations;

namespace TeamcollborationHub.server.Entities.Dto;

/// <summary>
/// 
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record  UserRequestDto(
   [EmailAddress] [Required] string Email,
    string Password
);

public record AuthenticationResponse(
    string email,
    string AccessToken,
    int ExpiryDate
);

/// <summary>
/// 
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="UserName"></param>
public record  CreateUserDto(
    [EmailAddress] [Required] string Email,
    string Password,
    [StringLength(20, MinimumLength = 4)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$",
        ErrorMessage = "Username can contain only letters, numbers, and underscores")]
    string UserName);