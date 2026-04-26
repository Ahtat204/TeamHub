using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.SetProjectDeadline;

//TODO:this Operation should require Authorization,just like the other project creation and modification operations
public record SetProjectDeadlineCommand(int ProjectId, DateTime Deadline) : IRequest<Project>;