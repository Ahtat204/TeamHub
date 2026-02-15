using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(TDBContext context): IRequestHandler<GetProjectByIdQuery, Project>
{
    // TODO:Add NotFoundException<Project> in the GlobalException Handler ,better do it before merging to test its functionality
    public async Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var result = context.Projects.FirstOrDefault(pr => pr.Id == request.Id);
        if (result is null) throw new NotFoundException<Project>();
        return await Task.FromResult(result);
    }
}