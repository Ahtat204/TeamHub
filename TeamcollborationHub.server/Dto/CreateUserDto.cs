using System.ComponentModel.DataAnnotations;

namespace TeamcollborationHub.server.Dto;

public record CreateUserDto(
    [EmailAddress, Required] string Email,
    string Password,
    [StringLength(20, MinimumLength = 4), RegularExpression(@"^[a-zA-Z0-9_]+$",
         ErrorMessage = "Username can contain only letters, numbers, and underscores")]
    string UserName);
    
