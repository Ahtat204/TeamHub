using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Mediator.Queries;

public record GetProjectQuery(int Id): IRequest<Project>;