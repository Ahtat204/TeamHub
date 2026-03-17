
using dotenv.net.Utilities;
using IHostingEnvironment= Microsoft.Extensions.Hosting.IHostEnvironment ;


namespace TeamcollborationHub.server.Helpers;

public static class LoadValues
{
    public static string? LoadEnv(string key) => EnvReader.HasValue(key) ? EnvReader.GetStringValue(key) : Environment.GetEnvironmentVariable(key);
    public static string? LoadValue(string key, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment==Environments.Development)
        {
            return configuration.GetValue<string>(key);
        }
        return LoadEnv(key);
    }
    public static bool IsTesting(this IWebHostEnvironment hostEnvironment)=>hostEnvironment.IsEnvironment("Testing");
}