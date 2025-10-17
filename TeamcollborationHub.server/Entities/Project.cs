using System.ComponentModel.DataAnnotations;

namespace TeamcollborationHub.server.Entities;

    public class Project
    {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public required User Contributor { get; set; }
    public ProjectStatus status { get; set; } = ProjectStatus.NotStarted;

}

public enum ProjectStatus
{
    NotStarted,
    InProgress,
    Completed
}