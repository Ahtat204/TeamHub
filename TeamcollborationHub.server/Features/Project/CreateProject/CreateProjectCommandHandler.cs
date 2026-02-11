using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Project.CreateProject;

public class CreateProjectCommandHandler(TDBContext dbContext) : IRequestHandler<CreateProjectCommand, Entities.Project>
{
    public async Task<Entities.Project> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        Entities.Project project = new()
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