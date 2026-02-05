using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Caching;

public interface ICachingService<T,TKey>
{
    T? GetProjectFromCache(TKey projectId);
    void SetProjectInCache(TKey key, T project);
    public void EvictProjectFromCache(TKey key);
}