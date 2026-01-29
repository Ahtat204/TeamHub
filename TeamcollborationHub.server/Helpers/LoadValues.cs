namespace TeamcollborationHub.server.Helpers;

public static class LoadValues
{
    public static string? LoadEnv(string key)=>Environment.GetEnvironmentVariable(key);

    public static string? LoadValue(string key, IConfiguration configuration)
    {
        var env=LoadEnv(key);
        return env ?? configuration.GetValue<string>(key);
    }
    
}