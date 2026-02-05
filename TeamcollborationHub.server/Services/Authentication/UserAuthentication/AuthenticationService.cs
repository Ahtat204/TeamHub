using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Security;

namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

public class AuthenticationService(
    IPasswordHashingService passwordHashingService,
    IUserRepository authenticationRepository
   ) : IAuthenticationService
{
    public async Task<User?> AuthenticateUser(LoginRequestDto userRequest)
    {
        var email=userRequest.Email.Trim().ToLower();
        var user = await authenticationRepository.GetUserByEmail(email);
        if (user is null) throw new NotFoundException<User>();
        var verified = passwordHashingService.VerifyPassword(userRequest.Password, user.Password);
        if (!verified) throw new NotFoundException<User>("due to incorrect password");
        return user;
    }

    public async Task<User> CreateUser(CreateUserDto user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        var emailexist = await authenticationRepository.GetUserByEmail(user.Email);
        if (emailexist is not null) throw new Exception("this email already exist");
        var hashedPassword = passwordHashingService.Hash(user.Password);
        var savedUser = new User
        {
            Email = user.Email.Trim().ToLower(),
            Password = hashedPassword,
            Name = user.UserName.Trim()
        };
        var result= await authenticationRepository.CreateUser(savedUser);
        return result ?? new User
        {
            Name = user.UserName,
            Email = user.Email,
        };
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