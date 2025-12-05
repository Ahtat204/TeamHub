using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

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
    public IQueryable<Project> GetAllProjects()
    {
        return _dbContext.Projects;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public  async Task<Project?> GetProjectByName(string name)
    {
        return await _dbContext.Projects.FindAsync(name);
    }

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