using MediatR;
using TeamcollborationHub.server.Configuration;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;

public class RemoveProjectTaskHandler(TdbContext db) : IRequestHandler<RemoveProjectTaskCommand>
{
    public async Task Handle(RemoveProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var target = db.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (target is null) return;
        db.Remove(target);
        await db.SaveChangesAsync(cancellationToken);
    }
}