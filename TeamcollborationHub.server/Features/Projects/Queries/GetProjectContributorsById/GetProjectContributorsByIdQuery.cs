using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectContributorsById;

public record GetProjectContributorsByIdQuery(int ProjectId) : IRequest<IEnumerable<User>>;