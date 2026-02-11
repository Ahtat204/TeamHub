using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;

namespace TeamcollborationHub.server.Repositories.ProjectRepository;

/// <summary>
///TODO :this is not gonna be used ,Initially I thought about using the same pattern for the project table , but creating a project record is too complex , and I decided to use CQRS since it seperates reads and writes, which exactly what's needed here, as fecthing a project record , or even all of them is a lot simpler than creating one 
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

    public User? AddContributorToProject(int projectId, int userId)
    {
        var result =  _dbContext.Users.FirstOrDefault(u => u.Id == userId);
        if (result is null) return null;
        _dbContext.Users.Add(result);
        _dbContext.SaveChanges();
        return result;
    }

    public  void RemoveContributorFromProject(int projectId, int userId) 
    {
        var result= _dbContext.Users.FirstOrDefault(u => u.Id == userId && u.ProjectId == projectId);
        if (result is null) return;
        _dbContext.Users.Remove(result);
        dbContext.SaveChanges();
    }
    
    public void AddProjectTask(int id,ProjectTask projectTaskId)
    {
       var result= _dbContext.Projects.FirstOrDefault(pr => pr.Id == id);
       result?.Tasks?.Add(projectTaskId);
    }
    public int RemoveProjectTask(int projectTaskId) => _dbContext.Projects.Where(pr=>pr.Id==projectTaskId).ExecuteDelete();
    public int SetProjectStartDate(int projectId, DateTime startDate)=>_dbContext.Projects.Where(p=>p.Id==projectId).ExecuteUpdate(u=>u.SetProperty(pr=>pr.Deadline, startDate));
    public int SetProjectEndDate(int projectId, DateTime endDate)=>_dbContext.Projects.Where(p=>p.Id==projectId).ExecuteUpdate(u=>u.SetProperty(pr=>pr.Deadline, endDate));
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