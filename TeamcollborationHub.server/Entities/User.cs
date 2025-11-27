using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamcollborationHub.server.Entities;
/// <summary>
/// 
/// </summary>
    public class User
    {
    [Key]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }= string.Empty;
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }=string.Empty;
    [Required]
    public string Password { get; set; }=string.Empty;
    public Project? project { get; set; }
    public int projectId { get; set; }

}

