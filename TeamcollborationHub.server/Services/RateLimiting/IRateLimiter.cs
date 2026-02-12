namespace TeamcollborationHub.server.Services.RateLimiting
{
    public interface IRateLimiter
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next);
    }
}
