using TeamcollborationHub.server.Endpoints;
using TeamcollborationHub.server.Helpers;

namespace TeamcollborationHub.server.Middlewares;

public static class Register
{
    public static WebApplication RegisterMiddlewares(this WebApplication app)
    {
        app.MapEndpoints();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseExceptionHandler();
        if (app.Environment.IsTesting())
        {
            app.Use(async (context, next) =>
            {
                context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
                await next();
            });
        }
        app.UseMiddleware<IpBasedRateLimiter>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}