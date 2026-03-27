
using dotenv.net.Utilities;


namespace TeamcollborationHub.server.Helpers;

public static class LoadValues //thinking about a better name for this class
{
    public static string? LoadEnv(string key) => EnvReader.HasValue(key) ? EnvReader.GetStringValue(key) : Environment.GetEnvironmentVariable(key);
    public static string? LoadValue(string key, IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return environment == Environments.Development ? configuration.GetValue<string>(key) : LoadEnv(key);
    }
    public static bool IsTesting(this IWebHostEnvironment hostEnvironment) => hostEnvironment.IsEnvironment("Testing");
}