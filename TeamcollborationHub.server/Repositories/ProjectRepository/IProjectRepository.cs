using TeamcollborationHub.server.Enums;


namespace TeamcollborationHub.server.Repositories.ProjectRepository;

public interface IProjectRepository
{
    
    
   
    public void RemoveContributorFromProject(int projectId, int userId);
    public int RemoveProjectTask(int projectTaskId);
    public int SetProjectStartDate(int projectId, DateTime startDate);
    public int SetProjectEndDate(int projectId, DateTime endDate);
    public int SetProjectStatus(int projectId, ProjectStatus status);
    
    
}
