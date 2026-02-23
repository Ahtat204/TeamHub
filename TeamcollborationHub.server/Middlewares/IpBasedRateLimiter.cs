using Microsoft.AspNetCore.Mvc.ModelBinding;
using StackExchange.Redis;
using TeamcollborationHub.server.Exceptions;


namespace TeamcollborationHub.server.Middlewares
{
    public class IpBasedRateLimiter
    {
        private readonly ILogger<IpBasedRateLimiter> _logger;
        private readonly LuaScript _luaScript;
        private readonly IDatabase _redisDatabase;
        private readonly RequestDelegate next;

        public IpBasedRateLimiter(ILogger<IpBasedRateLimiter> logger, LuaScript luaScript, IDatabase redisDatabase, RequestDelegate next)
        {
            _logger = logger;
            _luaScript = luaScript;
            _redisDatabase = redisDatabase;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_luaScript is null || _redisDatabase is null)
                throw new ValueProviderException("service wasn't properly configured");
            var ip = context.Connection.RemoteIpAddress?.ToString() ??
                     throw new NotFoundException<string>("Unable to determine client IP address.");
            var allowed = (int)await _redisDatabase.ScriptEvaluateAsync(_luaScript.ToString(), [ip], [10, 1]);
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

    public static class RegisterMiddleware
    {
        public static IApplicationBuilder UseIpBasedRateLimiter(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IpBasedRateLimiter>();
        }
    }
}