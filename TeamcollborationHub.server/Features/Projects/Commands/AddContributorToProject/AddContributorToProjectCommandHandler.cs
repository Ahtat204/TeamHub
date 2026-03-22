using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;

public class AddContributorToProjectCommandHandler(TdbContext db) : IRequestHandler<AddContributorToProjectCommand, Project> 
{
    public async Task<Project> Handle(AddContributorToProjectCommand request, CancellationToken cancellationToken) // since EFcore.InMemory Doesn't support ExecuteUpdateAsync , this function will not be tested , can't just use another updating approach , because this the optimal and modern one
    {
        var result = db.Projects.FromSqlInterpolated(
                $@"
        UPDATE [user]
        SET ProjectId = {request.ProjectId}
        WHERE Id = {request.UserId};

        SELECT * FROM Project WHERE Id = {request.ProjectId};
    ")
            .AsEnumerable().First();
        await db.SaveChangesAsync(cancellationToken);
        if (result is null) throw new NotFoundException<Project>();
        return result;
    }
}