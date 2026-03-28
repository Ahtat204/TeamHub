using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamcollborationHub.server.Entities;

/// <summary>
/// 
/// </summary>
[Table("user")]
public class User
{
    [Key] public int Id { get; set; }
    [Column("name"), StringLength(100)] public string Name { get; set; } = string.Empty;
    [EmailAddress(ErrorMessage = "Invalid email format."), Required, StringLength(100)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(100)] public string Password { get; set; } = string.Empty;
    [Column("ProjectId")]
    public int? ProjectId = null;
    public Project? project = null;
}