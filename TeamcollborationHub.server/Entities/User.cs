using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamcollborationHub.server.Entities;

    public class User
    {
    [Key]
    public int Id { get; set; }
    [Column("nvarchar(20)")]
    public string Name { get; set; }= string.Empty;
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }=string.Empty;
    [Required]
    public string Password { get; set; }=string.Empty;
    public Project project { get; set; }
    public int projectId { get; set; }

}

