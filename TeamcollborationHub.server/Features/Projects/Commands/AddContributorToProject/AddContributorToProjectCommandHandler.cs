using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;

public class AddContributorToProjectCommandHandler(TdbContext db)
    : IRequestHandler<AddContributorToProjectCommand, Project>
{
    public async Task<Project>
        Handle(AddContributorToProjectCommand request,
            CancellationToken cancellationToken) // since EFcore.InMemory Doesn't support ExecuteUpdateAsync , this function will not be tested , can't just use another updating approach , because this the optimal and modern one
    {
        var user = await db.Users.FindAsync([request.UserId,cancellationToken], cancellationToken)
                   ?? throw new NotFoundException<User>();

        user.ProjectId = request.ProjectId;
        await db.SaveChangesAsync(cancellationToken);
        var project = await db.Projects
                          .Include(p => p.Contributors) // optional, if you want contributors
                          .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
                      ?? throw new NotFoundException<Project>();

        return project;
    }
}