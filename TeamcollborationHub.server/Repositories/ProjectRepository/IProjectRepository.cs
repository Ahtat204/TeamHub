using System.Runtime.InteropServices.JavaScript;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;


namespace TeamcollborationHub.server.Repositories.ProjectRepository;

public interface IProjectRepository
{
    public Task<Project?> GetProjectById(int id);
    public IQueryable<Project> GetAllProjects();
    public Task<Project> UpdateProject(int id,Project project);
    public Task<User?> AddContributorToProject(int projectId, int userId);
    public Task RemoveContributorFromProject(int projectId, int userId);
    public int AddProjectTask(ProjectTask projectTaskId);
    public int RemoveProjectTask(int projectTaskId);
    public int SetProjectStartDate(int projectId, DateTime startDate);
    public int SetProjectEndDate(int projectId, DateTime endDate);
    public int SetProjectStatus(int projectId, ProjectStatus status);
    public IQueryable<Project>? SearchProjects(string searchTerm);
    public IQueryable<ProjectTask> GetProjectTasks(int projectId);
    
    
}
