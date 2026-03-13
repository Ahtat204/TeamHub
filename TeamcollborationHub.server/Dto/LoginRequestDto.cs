using System.ComponentModel.DataAnnotations;

namespace TeamcollborationHub.server.Dto;

/// <summary>
/// 
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record LoginRequestDto(
    [EmailAddress] [Required] string Email,
    string Password
);
