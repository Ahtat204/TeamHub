using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveContributorFromProject;

public class RemoveContributorFromProjectCommandHandler(TdbContext db) : IRequestHandler<RemoveContributorFromProjectCommand,Project?>
{
    public async Task<Project> Handle(RemoveContributorFromProjectCommand request, CancellationToken cancellationToken)
    {
        var result = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)??throw new NotFoundException<User>();
                result.ProjectId = null;
                result.project = null;
                await db.SaveChangesAsync(cancellationToken);
            var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)??throw new NotFoundException<Project>();
            return project;
    }
}