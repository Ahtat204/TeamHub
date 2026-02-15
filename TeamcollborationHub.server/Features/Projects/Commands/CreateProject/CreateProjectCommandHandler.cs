using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(TdbContext dbContext) : IRequestHandler<CreateProjectCommand, Project>
{
    public async Task<Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        Project project = new()
        {
            Name = request.Name,
            Description = request.Description,
            Deadline = request.Deadline,
            status = request.ProjectStatus
        };
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(project);
    }
}