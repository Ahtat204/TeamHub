using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveContributorFromProject;

public class RemoveContributorFromProjectCommandHandler(TdbContext db) : IRequestHandler<RemoveContributorFromProjectCommand>
{
    public async Task Handle(RemoveContributorFromProjectCommand request, CancellationToken cancellationToken)
    {
        var result = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (result is not null)
        {
            if (result.ProjectId == request.ProjectId)
            {
                result.ProjectId = null;
                result.project = null;
            }
        }
        await db.SaveChangesAsync(cancellationToken);
    }
}