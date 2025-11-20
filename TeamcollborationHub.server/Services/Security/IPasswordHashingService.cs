namespace TeamcollborationHub.server.Services.Security;

public interface IPasswordHashingService
{
    public string Hash(string password);
    bool VerifyPassword(string password, string passwordHash);
}
