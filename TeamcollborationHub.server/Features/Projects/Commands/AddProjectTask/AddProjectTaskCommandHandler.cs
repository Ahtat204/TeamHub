using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;

public class AddProjectTaskCommandHandler(TdbContext db) : IRequestHandler<AddProjectTaskCommand, Project>
{
    public async Task<Project> Handle(AddProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var result = db.Projects.FirstOrDefault(pr => pr.Id == request.ProjectId);
        if (result is null) throw new NotFoundException<Project>();
        request.task.projectId = result.Id;
        result.Tasks?.Add(request.task);
        await db.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(result);
    }
}