namespace TeamcollborationHub.server.Services.Caching;

    public interface ICachingService<T>
    {
        T? GetProjectFromCache(int projectId);
        void SetProjectInCache(string key,T value);
    }

