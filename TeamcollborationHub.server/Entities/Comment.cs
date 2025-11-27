namespace TeamcollborationHub.server.Entities;
/// <summary>
/// 
/// </summary>
public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Project Project { get; set; }
    public int projectId { get; set; }

}