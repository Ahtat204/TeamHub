using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;

public record AddProjectTaskCommand(int ProjectId, ProjectTask task) : IRequest<ProjectTask>;