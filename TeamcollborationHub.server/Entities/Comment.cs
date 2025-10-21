namespace TeamcollborationHub.server.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int ProjectId { get; set; }

}