using MediatR;

namespace TeamcollborationHub.server.Features.Project.GeProject;

public record GetProjectQuery(int Id): IRequest<Entities.Project>;