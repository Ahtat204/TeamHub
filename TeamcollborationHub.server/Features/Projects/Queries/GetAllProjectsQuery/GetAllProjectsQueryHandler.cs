using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectsQuery
{
    public class GetAllProjectsQueryHandler(TDBContext dBContext) : IRequestHandler<GetAllProjectsQuery, IEnumerable<Project>>
    {
        public async Task<IEnumerable<Project>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
         return await (dBContext.Projects.AsEnumerable().ToList().AsReadOnly() is IEnumerable<Project> projects
             ? Task.FromResult(projects)
             : Task.FromResult(Enumerable.Empty<Project>()));
        }
    }
}
