using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectComments;

public class GetAllProjectCommentsQueryHandler(TdbContext db):IRequestHandler<GetAllProjectCommentsQuery,IEnumerable<Comment>>
{
    public async Task<IEnumerable<Comment>> Handle(GetAllProjectCommentsQuery request, CancellationToken cancellationToken)
    {
        var project=await db.Projects.AsNoTracking().Where(u=>u.Id==request.ProjectId).Include(pr=>pr.Comments).FirstOrDefaultAsync(cancellationToken: cancellationToken) ??throw new NotFoundException<Project>();
        return project.Comments ?? [];
    }
}