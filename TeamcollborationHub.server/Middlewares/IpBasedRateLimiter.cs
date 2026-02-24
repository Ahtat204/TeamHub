using Microsoft.AspNetCore.Mvc.ModelBinding;
using StackExchange.Redis;
using TeamcollborationHub.server.Exceptions;


namespace TeamcollborationHub.server.Middlewares;

internal class IpBasedRateLimiter(
    ILogger<IpBasedRateLimiter> logger,
    LuaScript luaScript,
    IDatabase redisDatabase,
    RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (luaScript is null || redisDatabase is null)
            throw new ValueProviderException("service wasn't properly configured");
        var ip = context.Connection.RemoteIpAddress?.ToString() ??
                 throw new NotFoundException<string>("Unable to determine client IP address.");
        var allowed = (int)await redisDatabase.ScriptEvaluateAsync(luaScript.ToString() ?? throw new InvalidOperationException(),
                [ip], [10, 1]);
        if (allowed == 0)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests. Please try again later.");
            logger.LogWarning("Rate limit exceeded for IP: {IP}", ip);
            return;
        }

        await next(context);
    }
}