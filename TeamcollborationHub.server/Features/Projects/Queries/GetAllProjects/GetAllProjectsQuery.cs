using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

public record GetAllProjectsQuery() : IRequest<List<Project>>;