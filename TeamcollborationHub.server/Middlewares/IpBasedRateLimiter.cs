using System.Net;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StackExchange.Redis;
using TeamcollborationHub.server.Exceptions;
namespace TeamcollborationHub.server.Middlewares;
/// <summary>
/// Middleware that enforces IP-based rate limiting using Redis and Lua scripting.
/// </summary>
/// <remarks>
/// This component intercepts incoming HTTP requests, tracks request counts per client IP,
/// and enforces a maximum request threshold within a fixed expiry window (default: 60 seconds).
/// If the threshold is exceeded, the middleware returns a 429 Too Many Requests response.
/// </remarks>
public class IpBasedRateLimiter
{
  /// <summary>
    /// Logger used to record rate limiting events and warnings.
    /// </summary>
    private readonly ILogger<IpBasedRateLimiter> _logger;

    /// <summary>
    /// Lua script used to atomically evaluate request counts in Redis.
    /// </summary>
    private readonly LuaScript _luaScript;

    /// <summary>
    /// Redis database instance used to store and evaluate request counters.
    /// </summary>
    private readonly IDatabase _redisDatabase;

    /// <summary>
    /// Delegate representing the next middleware in the pipeline.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Maximum number of requests allowed per IP within the expiry window.
    /// </summary>
    private readonly int _maxRequests;

    /// <summary>
    /// Application configuration source used to retrieve rate limiting settings.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Expiry window (in seconds) for rate limiting counters. Default is 60 seconds.
    /// </summary>
    private const int Expiry = 60;

    /// <summary>
    /// Initializes a new instance of the <see cref="IpBasedRateLimiter"/> class.
    /// </summary>
    /// <param name="logger">Logger for recording rate limiting events.</param>
    /// <param name="luaScript">Lua script for atomic Redis evaluation.</param>
    /// <param name="redisDatabase">Redis database instance for request tracking.</param>
    /// <param name="next">Delegate to invoke the next middleware in the pipeline.</param>
    /// <param name="configuration">Configuration source for rate limiting settings.</param>
    public IpBasedRateLimiter(ILogger<IpBasedRateLimiter> logger,
        LuaScript luaScript,
        IDatabase redisDatabase,
        RequestDelegate next,
        IConfiguration configuration)
    {
        _logger = logger;
        _luaScript = luaScript;
        _redisDatabase = redisDatabase;
        _next = next;
        _configuration = configuration;
        _maxRequests = _configuration.GetValue<int>("maxReq");
    }

    /// <summary>
    /// Invokes the middleware for the given HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP request context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ValueProviderException">Thrown if Redis or Lua script is not properly configured.</exception>
    /// <exception cref="NotFoundException{IPAddress}">Thrown if the client IP address cannot be determined.</exception>
    /// <remarks>
    /// If the request count for the client IP exceeds the configured threshold,
    /// the middleware responds with HTTP 429 Too Many Requests and logs a warning.
    /// Otherwise, the request is passed to the next middleware in the pipeline.
    /// </remarks>

    public async Task InvokeAsync(HttpContext context)
    {
        if (_luaScript is null || _redisDatabase is null)
        {
            throw new ValueProviderException("service wasn't properly configured");
        }
        var ip = context.Connection.RemoteIpAddress?.ToString() ??
                 throw new NotFoundException<IPAddress>("Unable to determine client IP address.");
        RedisKey[] redisKey = [ip];
        string script = _luaScript.ExecutableScript;
        var res = (long)await _redisDatabase.ScriptEvaluateAsync(script, redisKey, [_maxRequests, Expiry]);
        if (res == 1)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests. Please try again later.");
            _logger.LogWarning("Rate limit exceeded for IP: {IP}", ip);
            return;
        }
        await _next(context);
    }
}