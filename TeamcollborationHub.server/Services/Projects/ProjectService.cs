using MediatR;
using TeamcollborationHub.server.Entities;


namespace TeamcollborationHub.server.Services.Projects;

public class ProjectService(IMediator mediator) : IProjectService
{
    public Task<Project> AddContributorToProject(int projectId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Project> AddProjectTask(int id, ProjectTask projectTask)
    {
        throw new NotImplementedException();
    }

    public Task<Project> CreateProject(Project project)
    {
        throw new NotImplementedException();
    }
    
    

    public Task<List<Project>> GetAllProjects()
    {
        throw new NotImplementedException();
    }
    
    public Task<Project> GetProjectById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetProjectContributorById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ProjectTask> GetProjectTaskById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Project> RemoveContributorFromProject(int projectId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<Project> RemoveProjectTask(int projectTaskId)
    {
        throw new NotImplementedException();
    }

    public Task<Project> SetProjectStartDate(int projectId, DateTime startDate)
    {
        throw new NotImplementedException();
    }
}
