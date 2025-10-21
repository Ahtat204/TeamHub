using System.ComponentModel.DataAnnotations;
using TeamcollborationHub.server.Enums;
namespace TeamcollborationHub.server.Entities;

    public class Project
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<User>  contributor { get; set; }
    public ProjectStatus status { get; set; } = ProjectStatus.NotStarted;
    public ICollection<Comment> comments { get; set; } 
    public ICollection<ProjectTask>? Tasks { get; set; } 

}

