using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TeamcollborationHub.server.Entities;

[Table("Task")]
public class ProjectTask
{
    [Key] public int Id { get; set; }

    [Length(3, 30), Required, Column("title")]
    public string Title { get; set; } = string.Empty;

    [Length(0, 1000), Column("description")]
    public string? Description { get; set; } = string.Empty;

    [DataType(DataType.Date)] public DateTime DueDate { get; init; }
    public DateTime StartedDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public Project project { get; set; }
    [Required] public int projectId { get; set; }
}