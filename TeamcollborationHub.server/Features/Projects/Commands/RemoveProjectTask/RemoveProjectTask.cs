using MediatR;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;

public record RemoveProjectTask(int ProjectTaskId) : IRequest<bool>;