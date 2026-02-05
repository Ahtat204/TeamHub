using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace TeamcollborationHub.server.Entities.Dto;

/// <summary>
/// 
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record LoginRequestDto(
    [EmailAddress] [Required] string Email,
    string Password
);

public record RefreshTokenDto(string Token, string Id);
public record LoginResponseDto(
    string email,
    string? AccessToken,
    int ExpiryDate,
    RefreshTokenDto? RefreshToken=null
);

public record RefreshAccessDto(string AccessToken,string RefreshToken);
public  record RegisterUserDto(string Email, string token);

/// <summary>
/// 
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="UserName"></param>
public record CreateUserDto(
    [EmailAddress, Required] string Email,
    string Password,
    [StringLength(20, MinimumLength = 4), RegularExpression(@"^[a-zA-Z0-9_]+$",
         ErrorMessage = "Username can contain only letters, numbers, and underscores")]
    string UserName);