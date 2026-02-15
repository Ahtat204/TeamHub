using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectTasks;

// TODO:Add ArgumentNullException in the GlobalException Handler 
public class GetAllProjectTasksQueryHandler(TDBContext db):IRequestHandler<GetAllProjectTasksQuery, IEnumerable<ProjectTask>?>//done
{
    public Task<IEnumerable<ProjectTask>?> Handle(GetAllProjectTasksQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var result= db.Tasks.Where(t=>t.projectId==request.ProjectId).AsEnumerable()?? [];
        return Task.FromResult(result)!;
    }
}