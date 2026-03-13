using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Caching;

/// <summary>
/// Provides Redis-based caching operations for <see cref="Project"/> entities.
/// </summary>
/// <remarks>
/// This service uses <see cref="IDistributedCache"/> as a backing store
/// and serializes cached values using JSON.
/// Cache entries use both absolute and sliding expiration policies.
/// </remarks>
    public class RedisCachingService(IDistributedCache? distributedCache) : ICachingService<Project,string>
    {
        /// <summary>
        /// Retrieves a project from the cache by its identifier.
        /// </summary>
        /// <param name="projectId">
        /// The cache key associated with the project.
        /// </param>
        /// <returns>
        /// The cached <see cref="Project"/> if found;
        /// otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// If the cache entry does not exist or the cache provider
        /// is unavailable, this method returns <c>null</c>.
        /// </remarks>
        public Project? GetProjectFromCache(string projectId)
        {
            var data = distributedCache?.GetString(projectId.ToString());
            return data == null ? null : JsonSerializer.Deserialize<Project>(data)!;
        }
        /// <summary>
        /// Stores a project in the cache.
        /// </summary>
        /// <param name="key">
        /// The cache key under which the project is stored.
        /// </param>
        /// <param name="project">
        /// The project instance to cache.
        /// </param>
        /// <remarks>
        /// Cached entries expire after a fixed duration and may also
        /// be extended through sliding expiration if accessed frequently.
        /// </remarks>
        public void SetProjectInCache(string key, Project project)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            distributedCache?.SetString(key, JsonSerializer.Serialize(project), options);
        }
        /// <summary>
        /// Removes a project entry from the cache.
        /// </summary>
        /// <param name="key">
        /// The cache key identifying the project to remove.
        /// </param>
        /// <remarks>
        /// This operation is safe to call even if the key does not exist.
        /// </remarks>
        public void EvictProjectFromCache(string key)
        {
            distributedCache?.Remove(key);
        }
    }
    /// <summary>
    /// Provides Redis-based caching operations for <see cref="RefreshToken"/> entities.
    /// </summary>
    /// <remarks>
    /// This service is responsible for caching refresh tokens to reduce
    /// database lookups during authentication flows.
    /// </remarks>
    public class RefreshTokenCachingService(IDistributedCache? distributedCache) : ICachingService<RefreshToken, string>
    {
        /// <summary>
        /// Retrieves a refresh token from the cache.
        /// </summary>
        /// <param name="projectId">
        /// The cache key associated with the refresh token.
        /// </param>
        /// <returns>
        /// The cached <see cref="RefreshToken"/> if found;
        /// otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Absence from cache does not imply invalidity of the token;
        /// it only indicates that the token is not cached.
        /// </remarks>
        public RefreshToken? GetProjectFromCache(string projectId)
        {
            var token = distributedCache?.GetString(projectId.ToString());
            return token == null ? null : JsonSerializer.Deserialize<RefreshToken>(token)!;
        }

        /// <summary>
        /// Stores a refresh token in the cache.
        /// </summary>
        /// <param name="key">
        /// The cache key under which the refresh token is stored.
        /// </param>
        /// <param name="project">
        /// The refresh token instance to cache.
        /// </param>
        /// <remarks>
        /// Cache expiration policies mirror those used for project caching
        /// to ensure consistency across cache layers.
        /// </remarks>
        public void SetProjectInCache(string key, RefreshToken project)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            distributedCache?.SetString(key, JsonSerializer.Serialize(project), options);
        }

        /// <summary>
        /// Removes a refresh token from the cache.
        /// </summary>
        /// <param name="key">
        /// The cache key identifying the refresh token to remove.
        /// </param>
        /// <remarks>
        /// This method is typically invoked during token revocation
        /// or logout workflows.
        /// </remarks>
        public void EvictProjectFromCache(string key)
        {
            distributedCache?.Remove(key);
        }
    }

