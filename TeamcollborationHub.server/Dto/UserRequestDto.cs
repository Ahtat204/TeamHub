using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TeamcollborationHub.server.Dto;

public record UserRequestDto(
    [EmailAddress, Required(ErrorMessage = "Email is Required")] string Email,
    [Category("Security")
     , Description("User's password.")
     , PasswordPropertyText(true),DataType(DataType.Password)]
    string Password
);