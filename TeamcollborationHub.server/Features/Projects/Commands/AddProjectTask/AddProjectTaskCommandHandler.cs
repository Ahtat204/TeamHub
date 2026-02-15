using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;

public class AddProjectTaskCommandHandler(TdbContext db) : IRequestHandler<AddProjectTaskCommand, ProjectTask>
{
    public Task<ProjectTask> Handle(AddProjectTaskCommand request, CancellationToken cancellationToken)
    {
        var result = db.Projects.FirstOrDefault(pr => pr.Id == request.ProjectId);
        result?.Tasks?.Add(request.task);
        return Task.FromResult(request.task);
    }
}