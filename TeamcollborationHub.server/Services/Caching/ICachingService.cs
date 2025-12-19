using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Caching
{
    public interface ICachingService
    {
        Project? GetProjectFromCache(int projectId);

        void SetProjectInCache(string key, Project project);
    }
}
