using System.ComponentModel.DataAnnotations.Schema;

namespace TeamcollborationHub.server.Entities;
/// <summary>
/// 
/// </summary>

[Table("Comment")]
public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Project Project { get; set; }
    public int projectId { get; set; }

}