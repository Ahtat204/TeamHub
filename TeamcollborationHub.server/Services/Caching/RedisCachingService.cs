using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Caching;

    public class RedisCachingService(IDistributedCache? distributedCache) : ICachingService<Project,string>
    {
        public Project? GetProjectFromCache(string projectId)
        {
            var data = distributedCache?.GetString(projectId.ToString());
            return data == null ? null : JsonSerializer.Deserialize<Project>(data)!;
        }
        public void SetProjectInCache(string key, Project project)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            distributedCache?.SetString(key, JsonSerializer.Serialize(project), options);
        }
        public void EvictProjectFromCache(string key)
        {
            distributedCache?.Remove(key);
        }
    }
    public class RefreshTokenCachingService(IDistributedCache? distributedCache) : ICachingService<RefreshToken, string>
    {
        public RefreshToken? GetProjectFromCache(string projectId)
        {
            var token = distributedCache?.GetString(projectId.ToString());
            return token == null ? null : JsonSerializer.Deserialize<RefreshToken>(token)!;
        }

        public void SetProjectInCache(string key, RefreshToken project)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            distributedCache?.SetString(key, JsonSerializer.Serialize(project), options);
        }

        public void EvictProjectFromCache(string key)
        {
            distributedCache?.Remove(key);
        }
    }

