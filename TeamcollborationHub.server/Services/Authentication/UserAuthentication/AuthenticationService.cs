using Microsoft.AspNetCore.Connections.Features;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories;
using TeamcollborationHub.server.Services.Security;
using TeamcollborationHub.server.Services.Authentication.Jwt;

namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

public class AuthenticationService(
    IConfiguration configuration,
    IPasswordHashingService passwordHashingService,
    AuthenticationRepository authenticationRepository,
    IJwtService jwtservice) : IAuthenticationService
{
    public async Task<AuthenticationResponse?> AuthenticateUser(UserRequestDto UserRequest)
    {
        var User = await authenticationRepository.GetUserByEmail(UserRequest.Email);
        if (User is null) throw new NotFoundException<User>();
        var verified = passwordHashingService.VerifyPassword(UserRequest.Password, User.Password);
        if (!verified) throw new NotFoundException<User>("due to incorrect password");
        var accesstoken = jwtservice.GenerateTokenResponse(User, out var tokenExpiryDate) ??
                          throw new Exception("Invalid credentials password");
        return new(
            email: UserRequest.Email, AccessToken: accesstoken, ExpiryDate: tokenExpiryDate);
    }

    public async Task<User?> CreateUser(CreateUserDto? user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        var emailexist = await authenticationRepository.GetUserByEmail(user.Email);
        if (emailexist is not null) throw new Exception("this email already exist");
        var hashedPassword = passwordHashingService.Hash(user.Password);
        var SavedUser = new User
        {
            Email = user.Email.Trim().ToLower(),
            Password = hashedPassword,
            Name = user.UserName.Trim()
        };
        return await authenticationRepository.CreateUser(SavedUser);
    }

    public async Task<User?> UpdateUser(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        var checkuser = await authenticationRepository.GetUserByEmail(user.Email);
        if (checkuser is null) throw new NotFoundException<User>("because email does not exist");
        checkuser.Email = user.Email.Trim().ToLower();
        checkuser.Password =checkuser.Password==user.Password?user.Password:passwordHashingService.Hash(user.Password);
        checkuser.Name = user.Name.Trim();
        return await authenticationRepository.UpdateUser(checkuser);
    }

    public async Task<User?> DeleteUser(int id)
    {
        return await authenticationRepository.deleteUser(id);
    }
}