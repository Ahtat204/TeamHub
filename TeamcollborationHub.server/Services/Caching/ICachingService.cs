namespace TeamcollborationHub.server.Services.Caching;

public interface ICachingService<T, in TKey>
{
    T? GetProjectFromCache(TKey projectId);
    void SetProjectInCache(TKey key, T project);
    public void EvictProjectFromCache(TKey key);
}