using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;

namespace TeamcollborationHub.server.Repositories.ProjectRepository;

/// <summary>
/// 
/// </summary>
/// <param name="dbContext"></param>
public class ProjectRepository(TDBContext dbContext) : IProjectRepository
{
    /// <summary>
    /// 
    /// </summary>
    private readonly TDBContext _dbContext = dbContext;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Project?> GetProjectById(int id)
    {
         return await  _dbContext.Projects.FindAsync(id);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IQueryable<Project> GetAllProjects()=>_dbContext.Projects;
    public Task<Project> UpdateProject(int id, Project project)
    {
        throw new NotImplementedException();
    }

    public Task<User?> AddContributorToProject(int projectId, int userId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveContributorFromProject(int projectId, int userId)
    {
        throw new NotImplementedException();
    }
    
    public int AddProjectTask(ProjectTask projectTaskId)
    {
        throw new NotImplementedException();
    }
    public int RemoveProjectTask(int projectTaskId) => _dbContext.Projects.Where(pr=>pr.Id==projectTaskId).ExecuteDelete();
    public int SetProjectStartDate(int projectId, DateTime startDate)=>_dbContext.Projects.Where(p=>p.Id==projectId).ExecuteUpdate(u=>u.SetProperty(pr=>pr.EndDateTime, startDate));
    public int SetProjectEndDate(int projectId, DateTime endDate)=>_dbContext.Projects.Where(p=>p.Id==projectId).ExecuteUpdate(u=>u.SetProperty(pr=>pr.EndDateTime, endDate));
    public int SetProjectStatus(int projectId, ProjectStatus status)=>_dbContext.Projects.Where(p => p.Id==projectId).ExecuteUpdate(s=>s.SetProperty(pr=>pr.status,status));
    public IQueryable<Project>? SearchProjects(string searchTerm)
    {
        throw new NotImplementedException();
    }
    public IQueryable<ProjectTask> GetProjectTasks(int projectId) => _dbContext.Projects.Where(p=>p.Id==projectId).SelectMany(p=>p.Tasks!);

    /// <summary>
    /// :
    /// </summary>
    /// <param name="project"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Project> UpdateProject(Project project)
    {
        _dbContext.Projects.Update(project);
        await _dbContext.SaveChangesAsync();
        return project;
    }
}