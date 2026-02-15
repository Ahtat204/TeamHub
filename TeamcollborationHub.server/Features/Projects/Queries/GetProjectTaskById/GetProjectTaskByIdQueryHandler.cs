using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectTaskById;

public class GetProjectTaskByIdQueryHandler(TdbContext db):IRequestHandler<GetProjectTaskByIdQuery, ProjectTask>
{
    public async Task<ProjectTask> Handle(GetProjectTaskByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var result =  await db.Tasks.FindAsync(new object?[] { request.Id, cancellationToken }, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (result is null) throw new NotFoundException<ProjectTask>();
        return result;
    }
}