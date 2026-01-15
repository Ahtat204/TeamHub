using Microsoft.AspNetCore.Connections.Features;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Security;
using TeamcollborationHub.server.Services.Authentication.Jwt;

namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IAuthenticationRepository _authenticationRepository;
    private readonly IJwtService _jwtservice;

    public AuthenticationService(IConfiguration configuration,
        IPasswordHashingService passwordHashingService,
        IAuthenticationRepository authenticationRepository,
        IJwtService jwtservice)
    {
        _passwordHashingService = passwordHashingService;
        _authenticationRepository = authenticationRepository;
        _jwtservice = jwtservice;
    }

    public AuthenticationService(PasswordHashing passwordHashing,IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
        _passwordHashingService = passwordHashing;
    }

    public AuthenticationService(IPasswordHashingService passwordHashing, IAuthenticationRepository authenticationRepository, IJwtService jwtservice)
    {
        passwordHashing = passwordHashing;
        _authenticationRepository = authenticationRepository;
        _jwtservice = jwtservice;
    }

    /// <summary>
/// for testing purposes
/// </summary>
/// <param name="passwordHashing"></param>
/// <param name="passwordHashingService"></param>
/// <param name="authenticationRepository"></param>
/// <exception cref="NotImplementedException"></exception>
 

    public async Task<AuthenticationResponse?> AuthenticateUser(UserRequestDto UserRequest)
    {
        var email=UserRequest.Email.Trim().ToLower();
        var User = await _authenticationRepository.GetUserByEmail(email);
        if (User is null) throw new NotFoundException<User>();
        var verified = _passwordHashingService.VerifyPassword(UserRequest.Password, User.Password);
        if (!verified) throw new NotFoundException<User>("due to incorrect password");
        var accesstoken = _jwtservice.GenerateTokenResponse(User, out var tokenExpiryDate) ??
                          throw new Exception("Invalid credentials password");
        return new(
            email: email, AccessToken: accesstoken, ExpiryDate: tokenExpiryDate);
    }

    public async Task<User?> CreateUser(CreateUserDto? user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        var emailexist = await _authenticationRepository.GetUserByEmail(user.Email);
        if (emailexist is not null) throw new Exception("this email already exist");
        var hashedPassword = _passwordHashingService.Hash(user.Password);
        var SavedUser = new User
        {
            Email = user.Email.Trim().ToLower(),
            Password = hashedPassword,
            Name = user.UserName.Trim()
        };
        return await _authenticationRepository.CreateUser(SavedUser);
    }

    public async Task<User?> UpdateUser(User user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        var checkuser = await _authenticationRepository.GetUserByEmail(user.Email);
        if (checkuser is null) throw new NotFoundException<User>("because email does not exist");
        checkuser.Email = user.Email.Trim().ToLower();
        checkuser.Password =checkuser.Password==user.Password?user.Password:_passwordHashingService.Hash(user.Password);
        checkuser.Name = user.Name.Trim();
        return await _authenticationRepository.UpdateUser(checkuser);
    }

    public async Task<User?> DeleteUser(int id)
    {
        return await _authenticationRepository.deleteUser(id);
    }
}