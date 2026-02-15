using System.ComponentModel.DataAnnotations;
using MediatR;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;

namespace TeamcollborationHub.server.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    [Required] string Name,
    string Description,
    IEnumerable<User>? Contributors,
    ProjectStatus? ProjectStatus,
    DateTime? Deadline) : IRequest<Project>;