using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Caching;

    public class RedisCachingService(IDistributedCache? distributedCache) : ICachingService
    {
        public Project? GetProjectFromCache(int projectId)
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

