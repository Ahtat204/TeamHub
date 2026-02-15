using MediatR;

namespace TeamcollborationHub.server.Features.Projects.Commands.SetProjectStartDate;

public record SetProjectStartDate(int ProjectId, DateTime StartDate) : IRequest<bool>;