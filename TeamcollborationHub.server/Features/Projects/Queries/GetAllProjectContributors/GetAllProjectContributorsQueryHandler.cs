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
            var result =dBContext.Projects.Where(pr => pr.Id == request.id);
            var allContributors = result.Select(proj => proj.Contributors).FirstOrDefault()?.AsEnumerable();
            return allContributors??([]);
        }
    }
}
