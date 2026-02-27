using Microsoft.AspNetCore.Mvc.ModelBinding;
using StackExchange.Redis;
using TeamcollborationHub.server.Exceptions;


namespace TeamcollborationHub.server.Middlewares;

public class IpBasedRateLimiter
{
    private readonly ILogger<IpBasedRateLimiter> _logger;
    private readonly LuaScript _luaScript;
    private readonly IDatabase _redisDatabase;
    private readonly RequestDelegate _next;

    public IpBasedRateLimiter(ILogger<IpBasedRateLimiter> logger,
        LuaScript luaScript,
        IDatabase redisDatabase, RequestDelegate next)
    {
        _logger = logger;
        _luaScript = luaScript;
        _redisDatabase = redisDatabase;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_luaScript is null || _redisDatabase is null)
        {
            throw new ValueProviderException("service wasn't properly configured");
        }
        var ip = context.Connection.RemoteIpAddress?.ToString() ??
                 throw new NotFoundException<string>("Unable to determine client IP address.");
        RedisKey[] redisKey = {ip};
        string script = _luaScript.ExecutableScript;
        var allowed = (long)await _redisDatabase.ScriptEvaluateAsync(script, [ip], [5, 1]);
        if (allowed == 0)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests. Please try again later.");
            _logger.LogWarning("Rate limit exceeded for IP: {IP}", ip);
            return;
        }
        _logger.LogInformation("the request was {request} and response is {response}",context.Request.Body,context.Response.Body );
        await _next(context);
    }
}
public static class RegisterMiddleware
{
    public static IApplicationBuilder UseIpBasedRateLimiter(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IpBasedRateLimiter>();
    }
}