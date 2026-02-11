using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

public interface IAuthenticationService
{
    public Task<AuthenticationResponse?> AuthenticateUser(UserRequestDto user);
    public Task<User?> CreateUser(CreateUserDto? user);
    public Task<User?> UpdateUser(User user);
    public Task<User?> DeleteUser(int id);
}