using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

public interface IAuthenticationService
{
    public Task<User?> AuthenticateUser(LoginRequestDto? user);
    public Task<User?> CreateUser(CreateUserDto user);
    public Task<User?> UpdatePassword(UpdatePasswordDto updatePasswordrequest);
    public Task<User?> DeleteUser(int id);
}