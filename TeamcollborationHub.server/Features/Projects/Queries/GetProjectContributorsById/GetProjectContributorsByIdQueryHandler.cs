using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectContributorsById;

public class GetProjectContributorsByIdQueryHandler(TdbContext db):IRequestHandler<GetProjectContributorsByIdQuery,IEnumerable<User>>
{
    public  Task<IEnumerable<User>> Handle(GetProjectContributorsByIdQuery request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        var result=db.Users.Where(pr=>pr.ProjectId==request.ProjectId).AsEnumerable();
        return Task.FromResult(result);
    }
}