using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Project.GeProject;

public class GetProjectQueryHandler(TDBContext context): IRequestHandler<GetProjectQuery, Entities.Project>
{
    public Task<Entities.Project> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var result = context.Projects.FirstOrDefault(pr => pr.Id == request.Id);
        if (result == null) throw new NotFoundException<Entities.Project>();
        return Task.FromResult(result);
    }
}