using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Caching
{
    public class RedisCachingService(IDistributedCache? distributedCache, ILogger<RedisCachingService> logger) : ICachingService<Project>
    {
       
        public Project? GetProjectFromCache(int projectId)
        {
            var data=distributedCache?.GetString(projectId.ToString());
            if (data is not null) return JsonSerializer.Deserialize<Project>(data)!;
            logger.LogWarning("Cache Miss");
            return  null;
        }

        public void SetProjectInCache(string key, Project project)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            distributedCache?.SetString(key,JsonSerializer.Serialize(project),options); 
        }
    }
}
