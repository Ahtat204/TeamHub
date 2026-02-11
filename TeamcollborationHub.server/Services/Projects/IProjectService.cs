using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Projects;

public interface IProjectService
{
    #region Queries
    public Task<List<Project>> GetAllProjects();
    public Task<Project> GetProjectById(int id);
    public Task<List<ProjectTask>>  GetAllProjectTasks();
    public Task<ProjectTask> GetProjectTaskById(int id);
    public Task<List<User>> GetAllProjectContributors();
    public Task<User> GetProjectContributorById(int id);

    #endregion
    #region Commands
    public Task<Project> CreateProject(Project project);
    public Task<Project> AddContributorToProject(int projectId, int userId);
    public Task<Project> RemoveContributorFromProject(int projectId, int userId);
    public Task<Project> AddProjectTask(int id,ProjectTask projectTask);
    public Task<Project> RemoveProjectTask(int projectTaskId);
    public Task<Project> SetProjectStartDate(int projectId, DateTime startDate);

    #endregion
}
