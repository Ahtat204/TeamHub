using System.ComponentModel.DataAnnotations;

namespace TeamcollborationHub.server.Dto;

/// <summary>
/// 
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public sealed record LoginRequestDto(
    [EmailAddress][Required] string Email,
    string Password
);
