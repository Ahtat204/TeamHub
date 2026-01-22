using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamcollborationHub.server.Enums;

namespace TeamcollborationHub.server.Entities;

/// <summary>
/// 
/// </summary>
[Table("Project")]
public class Project
{
    [Key] public int Id { get; set; }
    [Required] [StringLength(50)] 
    public required string Name { get; set; }
    [StringLength(300)] 
    public string Description { get; set; } = string.Empty;
    public ICollection<User>? contributor { get; set; }
    public ProjectStatus status { get; set; } = ProjectStatus.NotStarted;
    public ICollection<Comment>? comments { get; set; }
    public ICollection<ProjectTask>? Tasks { get; set; }
    [DataType(DataType.Date)] public DateTime? CreatedDateTime { get; set; }
    [DataType(DataType.Date)] public DateTime? LastModifiedDateTime { get; set; }
    [DataType(DataType.Date)] public DateTime? EndDateTime { get; set; }
}