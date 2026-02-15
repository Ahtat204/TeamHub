using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Mediator.Queries;

namespace TeamcollborationHub.server.Mediator.Handlers
{
    public class GetAllProjectContributorsQueryHandler(TDBContext dBContext) : IRequestHandler<GetAllProjectContributorsQuery, IEnumerable<User>>
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
