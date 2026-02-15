using MediatR;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;
 //TODO:this should require authorization
public record RemoveProjectTask(int ProjectTaskId) : IRequest<bool>;