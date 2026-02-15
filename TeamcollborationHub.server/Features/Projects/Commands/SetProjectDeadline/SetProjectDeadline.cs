using MediatR;

namespace TeamcollborationHub.server.Features.Projects.Commands.SetProjectDeadline;

public record SetProjectDeadline(int ProjectId, DateTime Deadline) : IRequest<DateTime>;