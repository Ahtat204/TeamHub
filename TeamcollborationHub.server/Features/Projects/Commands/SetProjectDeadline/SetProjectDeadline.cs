using MediatR;

namespace TeamcollborationHub.server.Features.Projects.Commands.SetProjectDeadline;

//TODO:this Operation should require Authorization,just like the other project creation and modification operations
public record SetProjectDeadline(int ProjectId, DateTime Deadline) : IRequest<DateTime>;