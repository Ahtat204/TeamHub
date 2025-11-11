using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Repositories;

namespace TeamcollborationHub.server.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationRepository _authenticationRepository;

    public AuthenticationService(AuthenticationRepository authenticationRepository)=>_authenticationRepository = authenticationRepository;
    

    public User? createUser()
    {
        throw new NotImplementedException();
    }

    public User? deleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public User? GetUserbyId()
    {
        throw new NotImplementedException();
    }

    public User? updateUser(User user)
    {
        throw new NotImplementedException();
    }
}
