using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;
//TODO:this should require authorization
public record RemoveProjectTaskCommand(int ProjectId,int TaskId) : IRequest<Project>;