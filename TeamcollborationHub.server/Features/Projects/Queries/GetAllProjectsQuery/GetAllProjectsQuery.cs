using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectsQuery;

public record GetAllProjectsQuery() : IRequest<List<Project>>;