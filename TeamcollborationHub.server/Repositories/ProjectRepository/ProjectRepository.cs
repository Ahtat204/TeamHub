using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Enums;

namespace TeamcollborationHub.server.Repositories.ProjectRepository;

/// <summary>
///TODO :this is not gonna be used ,Initially I thought about using the same pattern for the project table , but creating a project record is too complex , and I decided to use CQRS since it separates reads and writes, which exactly what's needed here, as fecthing a project record , or even all of them is a lot simpler than creating one 
/// </summary>
/// <param name="dbContext"></param>
public class ProjectRepository(TdbContext dbContext) : IProjectRepository
{
    public void RemoveContributorFromProject(int projectId, int userId) 
    {
        var result= dbContext.Users.FirstOrDefault(u => u.Id == userId && u.ProjectId == projectId);
        if (result is null) return;
        dbContext.Users.Remove(result);
        dbContext.SaveChanges();
    }
    public int RemoveProjectTask(int projectTaskId) => dbContext.Projects.Where(pr=>pr.Id==projectTaskId).ExecuteDelete();
    public int SetProjectStartDate(int projectId, DateTime startDate)=>dbContext.Projects.Where(p=>p.Id==projectId).ExecuteUpdate(u=>u.SetProperty(pr=>pr.Deadline, startDate));
    public int SetProjectEndDate(int projectId, DateTime endDate)=>dbContext.Projects.Where(p=>p.Id==projectId).ExecuteUpdate(u=>u.SetProperty(pr=>pr.Deadline, endDate));
    public int SetProjectStatus(int projectId, ProjectStatus status)=>dbContext.Projects.Where(p => p.Id==projectId).ExecuteUpdate(s=>s.SetProperty(pr=>pr.status,status));

    

}