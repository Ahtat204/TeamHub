using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Authentication;

    public interface IAuthenticationService
    {
    public User? GetUserbyId();
    public User? createUser();
    public User? updateUser(User user);
    public User? deleteUser(int id);
}

