namespace TeamcollborationHub.server.Exceptions;

/// <summary>
/// this exception is mean specifically for environment variables/Configuration values 
/// </summary>
/// <param name="key">the key/name of the variable </param>
public class ConfigurationValueMissingException(string key) : Exception($"Critical Configuration Missing: '{key}'. Check your .env file or Azure/AWS Key Vault.")
{
    public string Key { get; } = key;
}
