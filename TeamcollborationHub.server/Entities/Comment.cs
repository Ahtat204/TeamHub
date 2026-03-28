using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TeamcollborationHub.server.Entities;
/// <summary>
/// 
/// </summary>

[Table("Comment")]
public class Comment
{
    public int Id { get; init; }
    [StringLength(3000)]
    public string Content { get; set; } = string.Empty;
    [JsonIgnore]
    public Project Project { get; set; }
    public int projectId { get; set; }

}