using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectContributors
{
    public class GetAllProjectContributorsQueryHandler(TdbContext dBContext) : IRequestHandler<GetAllProjectContributorsQuery, IEnumerable<User>>
    {
        public async Task<IEnumerable<User>> Handle(GetAllProjectContributorsQuery request, CancellationToken cancellationToken)
        {
          var result= dBContext.Projects.Include(pro => pro.contributor).FirstOrDefault(pr => pr.Id == request.id) is { } project
                ? await Task.FromResult(project.contributor ?? Enumerable.Empty<User>())
                : await Task.FromResult(Enumerable.Empty<User>());
              return result;
        }
    }
}
