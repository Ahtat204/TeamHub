using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Repositories.ProjectRepository.ProjectTaskRepository;

public class TaskRepository: IProjectTaskRepository
{
    public IQueryable<ProjectTask> GetAllProjectTasks(int projectId)
    {
        throw new NotImplementedException();
    }
}