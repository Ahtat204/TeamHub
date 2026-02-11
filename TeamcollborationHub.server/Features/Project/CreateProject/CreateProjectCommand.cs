using System.ComponentModel.DataAnnotations;
using MediatR;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;


namespace TeamcollborationHub.server.Features.Project.CreateProject;

public record CreateProjectCommand(
    [Required] string Name,
    string Description,
    IEnumerable<User>? Contributors,
    ProjectStatus? ProjectStatus,
    DateTime? Deadline) : IRequest<Entities.Project>;
