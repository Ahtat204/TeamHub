using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;

public class AddContributorToProjectCommandHandler(TdbContext db) : IRequestHandler<AddContributorToProjectCommand, int>
{
    public async Task<int> Handle(AddContributorToProjectCommand request, CancellationToken cancellationToken) // since EFcore.InMemory Doesn't support ExecuteUpdateAsync , this function will not be tested , can't just use another updating approach , because this the optimal and modern one
    {
        var result = await db.Users.Where(u => u.Id == request.UserId).ExecuteUpdateAsync(setter => setter.SetProperty(u => u.ProjectId, request.ProjectId), cancellationToken: cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return result;
    }
}