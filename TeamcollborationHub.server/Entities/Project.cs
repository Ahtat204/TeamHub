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
    [Key] public int Id { get; init; }
    [Required] [StringLength(50)] 
    public required string Name { get; init; }
    [StringLength(300)] 
    public string Description { get; init; } = string.Empty;
    public ICollection<User>? contributor { get; init; }
    public ProjectStatus? status { get; init; } = ProjectStatus.NotStarted;
    public ICollection<Comment>? comments { get; init; }
    public ICollection<ProjectTask>? Tasks { get; init; }
    [DataType(DataType.Date)] public DateTime? CreatedDateTime { get; set; }
    [DataType(DataType.Date)] public DateTime? LastModifiedDateTime { get; set; }
    [DataType(DataType.Date)] public DateTime? Deadline { get; set; }
}