using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    public Project Project { get; init; }
    public int projectId { get; init; }

}