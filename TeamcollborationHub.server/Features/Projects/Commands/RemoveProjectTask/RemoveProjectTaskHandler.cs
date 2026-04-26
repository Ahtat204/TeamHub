using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;

public class RemoveProjectTaskHandler(TdbContext db) : IRequestHandler<RemoveProjectTaskCommand,Project>
{
    public async Task<Project> Handle(RemoveProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var target = db.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (target is null) throw new NotFoundException<ProjectTask>();
        db.Tasks.Remove(target);
        await db.SaveChangesAsync(cancellationToken);
        var result=db.Projects.FirstOrDefault(p => p.Id == request.ProjectId);
        return result ?? throw new NotFoundException<Project>();
    }
}