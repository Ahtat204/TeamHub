using StackExchange.Redis;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Services.RateLimiting
{
    public class IpBasedRateLimiter : IRateLimiter
    {
        private readonly ILogger<IpBasedRateLimiter> _logger;
       
        private readonly LuaScript luaScript;// this Lua script will be used to atomically check and update the request count for each IP address in Redis. It will return 1 if the request is allowed, and 0 if the rate limit has been exceeded.
        private readonly IDatabase _redisDatabase;
        private readonly int _maxRequests;// for more flexibility , I can make this configurable per endpoint or even per user in the future, but for now I will keep it simple and use a fixed value for all endpoints and users.
        private readonly int windowSizeInSeconds;// this is the time window for which the rate limit is applied. For example, if this is set to 60, then the rate limit will be applied for each 60 seconds window.

        public IpBasedRateLimiter(ILogger<IpBasedRateLimiter> logger, LuaScript luaScript, IDatabase redisDatabase, int maxRequests, int windowSizeInSeconds)
        {
            _logger = logger;
            this.luaScript = luaScript;
            _redisDatabase = redisDatabase;
            _maxRequests = maxRequests;
            this.windowSizeInSeconds = windowSizeInSeconds;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
           var ip =context.Connection.RemoteIpAddress?.ToString() ?? throw new NotFoundException<string>("Unable to determine client IP address.");
           var allowed=(int) await _redisDatabase.ScriptEvaluateAsync(luaScript.ToString(), new RedisKey[] { ip }, new RedisValue[] { _maxRequests, windowSizeInSeconds });
            if (allowed == 0) 
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                _logger.LogWarning("Rate limit exceeded for IP: {IP}", ip);
                return;
            }
            await next(context);
        }
    }
}
