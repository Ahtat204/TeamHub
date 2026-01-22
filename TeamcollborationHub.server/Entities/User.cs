using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamcollborationHub.server.Entities;

/// <summary>
/// 
/// </summary>
[Table("Task")]
public class User
{
    [Key] public int Id { get; init; }
    [Column("name")] 
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string Password { get; set; } = string.Empty;
    public int? ProjectId = null;
    public Project? project = null;
}