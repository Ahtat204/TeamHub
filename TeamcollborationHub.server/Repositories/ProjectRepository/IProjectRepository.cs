using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;


namespace TeamcollborationHub.server.Repositories.ProjectRepository;

public interface IProjectRepository
{
    public Task<Project?> GetProjectById(int id);
    public IQueryable<Project> GetAllProjects();
    public Task<Project?> GetProjectByName(string name);
    public Task<Project> UpdateProject(Project project);
    
}

public interface IProjectContributorRepository
{
    public Task<User?> AddContributorToProject(int projectId, int userId);
    public Task RemoveContributorFromProject(int projectId, int userId);
    public IQueryable<User?> GetAllContributorsInProject(int projectId);
    
}

public interface IProjectCommentRepository
{
    IQueryable<Comment?> GetAllProjectComments(int projectId);
}

public interface IProjectStatusRepository
{
    ProjectStatus GetProjectStatus(int projectId);
    public IQueryable<Project?> GetAllProjectWithStatus(ProjectStatus status);
}

public interface IProjectTaskRepository
{
    public IQueryable<ProjectTask> GetAllProjectTasks(int projectId);
    
    
}