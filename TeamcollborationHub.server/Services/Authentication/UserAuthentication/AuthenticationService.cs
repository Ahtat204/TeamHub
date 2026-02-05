using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Security;

namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

/// <summary>
/// Provides user authentication and account management operations.
/// </summary>
/// <remarks>
/// This service encapsulates business rules related to:
/// - User authentication
/// - User registration
/// - User updates
/// - User deletion
/// 
/// Security-sensitive operations such as password hashing and verification
/// are delegated to specialized services.
/// </remarks>
public class AuthenticationService(
    IPasswordHashingService passwordHashingService,
    IUserRepository authenticationRepository
   ) : IAuthenticationService
{
    /// <summary>
    /// Authenticates a user using login credentials.
    /// </summary>
    /// <param name="userRequest">
    /// The login request containing the user's email and password.
    /// </param>
    /// <returns>
    /// The authenticated <see cref="User"/> if credentials are valid.
    /// </returns>
    /// <exception cref="NotFoundException{User}">
    /// Thrown when the user does not exist or the password is incorrect.
    /// </exception>
    /// <remarks>
    /// The email is normalized before lookup.
    /// Password verification is performed using the password hashing service.
    /// </remarks>
    public async Task<User?> AuthenticateUser(LoginRequestDto userRequest)
    {
        var email=userRequest.Email.Trim().ToLower();
        var user = await authenticationRepository.GetUserByEmail(email);
        if (user is null) throw new NotFoundException<User>();
        var verified = passwordHashingService.VerifyPassword(userRequest.Password, user.Password);
        if (!verified) throw new NotFoundException<User>("due to incorrect password");
        return user;
    }

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="user">
    /// The data required to create a new user.
    /// </param>
    /// <returns>
    /// The newly created <see cref="User"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided user data is null.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when a user with the same email already exists.
    /// </exception>
    /// <remarks>
    /// The password is hashed before persistence.
    /// The email and username are normalized before storage.
    /// </remarks>
    public async Task<User> CreateUser(CreateUserDto user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));
        var emailexist = await authenticationRepository.GetUserByEmail(user.Email);
        if (emailexist is not null) throw new AlreadyExistsException<string>(user.Email);
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
    /// <summary>
    /// Updates an existing user account.
    /// </summary>
    /// <param name="user">
    /// The user entity containing updated values.
    /// </param>
    /// <returns>
    /// The updated <see cref="User"/> if the update succeeds;
    /// otherwise, <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided user is null.
    /// </exception>
    /// <exception cref="NotFoundException{User}">
    /// Thrown when no user exists with the specified email.
    /// </exception>
    /// <remarks>
    /// If the password has changed, it is re-hashed before persistence.
    /// Email and name values are normalized prior to update.
    /// </remarks>
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

    /// <summary>
    /// Deletes a user account by identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the user to delete.
    /// </param>
    /// <returns>
    /// The deleted <see cref="User"/> if found;
    /// otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This operation delegates deletion responsibility to the user repository.
    /// </remarks>
    public async Task<User?> DeleteUser(int id)
    {
        return await authenticationRepository.deleteUser(id);
    }
}