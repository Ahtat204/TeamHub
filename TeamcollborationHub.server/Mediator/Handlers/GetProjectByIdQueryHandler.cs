using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Mediator.Queries;

namespace TeamcollborationHub.server.Mediator.Handlers;

public class GetProjectByIdQueryHandler(TDBContext context): IRequestHandler<GetProjectByIdQuery, Project>
{
    public Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var result = context.Projects.FirstOrDefault(pr => pr.Id == request.Id);
        if (result == null) throw new NotFoundException<Entities.Project>();
        return Task.FromResult(result);
    }
}