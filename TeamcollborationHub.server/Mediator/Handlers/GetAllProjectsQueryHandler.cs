using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Mediator.Queries;

namespace TeamcollborationHub.server.Mediator.Handlers
{
    public class GetAllProjectsQueryHandler(TDBContext dBContext) : IRequestHandler<GetAllProjectsQuery, IEnumerable<Project>>
    {
        public Task<IEnumerable<Project>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
         return dBContext.Projects.AsEnumerable().ToList().AsReadOnly() is IEnumerable<Project> projects
             ? Task.FromResult(projects)
             : Task.FromResult(Enumerable.Empty<Project>());
        }
    }
}
