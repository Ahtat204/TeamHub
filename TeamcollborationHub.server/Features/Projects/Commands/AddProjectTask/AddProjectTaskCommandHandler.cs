using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;

public class AddProjectTaskCommandHandler(TdbContext db) : IRequestHandler<AddProjectTaskCommand, ProjectTask>
{
    public async Task<ProjectTask> Handle(AddProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var result = db.Projects.FirstOrDefault(pr => pr.Id == request.ProjectId);
        if (result is not null)
        {
            request.task.projectId = result.Id;
            result?.Tasks?.Add(request.task);
            await db.SaveChangesAsync(cancellationToken);
        }
        return await Task.FromResult(request.task);
    }
}