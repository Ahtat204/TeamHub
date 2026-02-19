using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;

public class RemoveProjectTaskHandler(TdbContext db) : IRequestHandler<RemoveProjectTaskCommand>
{
    public async Task Handle(RemoveProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var result = await  db.Tasks.Where(t=>t.Id==request.TaskId).ExecuteDeleteAsync(cancellationToken: cancellationToken);
    }
}