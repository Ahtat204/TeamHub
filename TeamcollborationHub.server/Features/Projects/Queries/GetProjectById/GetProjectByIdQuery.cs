using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(int Id) : IRequest<Project>;