using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Caching
{
    public class RedisCachingService : ICachingService
    {
        private readonly IDistributedCache? _distributedCache;

        public RedisCachingService(IDistributedCache? distributedCache)=> _distributedCache = distributedCache;
        public Project? GetProjectFromCache(int projectId)
        {
            var data=_distributedCache?.GetString(projectId.ToString());
            if(data == null) return default;
            return JsonSerializer.Deserialize<Project>(data)!;
        }

        public void SetProjectInCache(string key, Project project)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _distributedCache?.SetString(key,JsonSerializer.Serialize(project),options); 
        }
    }
}
