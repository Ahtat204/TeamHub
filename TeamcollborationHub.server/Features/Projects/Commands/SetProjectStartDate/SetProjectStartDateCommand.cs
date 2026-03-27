using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.SetProjectStartDate;

public record SetProjectStartDateCommand(int ProjectId, DateTime StartDate) : IRequest<Project>;