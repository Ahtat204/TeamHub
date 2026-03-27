using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;

public class AddProjectTaskCommandHandler(TdbContext db) : IRequestHandler<AddProjectTaskCommand, Project>
{
    public async Task<Project> Handle(AddProjectTaskCommand request, CancellationToken cancellationToken)
    {
        if (request.task is null) throw new ArgumentNullException(nameof(ProjectTask));
        var result = await db.Projects.FirstOrDefaultAsync(pr => pr.Id == request.ProjectId,cancellationToken: cancellationToken) ??
                     throw new NotFoundException<Project>();
        request.task.projectId = result.Id;
        request.task.project = result;
        db.Tasks.Add(request.task);
        await db.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(result);
    }
}