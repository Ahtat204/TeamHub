using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;

namespace TeamcollborationHub.server.Repositories.ProjectRepository.ProjectStatusRepository;

public class StatusRepository: IProjectStatusRepository
{
    public ProjectStatus GetProjectStatus(int projectId)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Project?> GetAllProjectWithStatus(ProjectStatus status)
    {
        throw new NotImplementedException();
    }
}