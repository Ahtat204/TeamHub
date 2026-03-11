namespace TeamcollborationHub.server.Middlewares;

public static class RegisterMiddleware
{
    public static IApplicationBuilder UseIpBasedRateLimiter(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<IpBasedRateLimiter>();
    }
}