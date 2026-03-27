using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(TdbContext context) : IRequestHandler<GetProjectByIdQuery, Project>
{
    // TODO:Add NotFoundException<Project> in the GlobalException Handler ,better do it before merging to test its functionality
    public async Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {

        Project? result = await context.Projects.AsNoTracking().FirstOrDefaultAsync(pr=>pr.Id == request.Id, cancellationToken);
        if (result is null) throw new NotFoundException<Project>(); //TODO:this will be handled in the Exception Handler Middleware
        return await Task.FromResult(result);
    }
}