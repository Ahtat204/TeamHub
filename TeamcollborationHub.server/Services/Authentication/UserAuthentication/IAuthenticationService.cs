using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;

namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

    public interface IAuthenticationService
    {
    public AuthenticationResponse? AuthenticateUser(UserRequestDto user);
    public User? createUser();
    public User? updateUser(User user);
    public User? deleteUser(int id);
}

