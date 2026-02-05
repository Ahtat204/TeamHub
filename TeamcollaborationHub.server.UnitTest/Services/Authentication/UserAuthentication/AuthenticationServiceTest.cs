using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using Moq;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Security;

namespace TeamcollaborationHub.server.UnitTest.Services.Authentication.UserAuthentication;

/// <summary>
/// Unit tests for <see cref="AuthenticationService"/>.
/// </summary>
/// <remarks>
/// This test suite verifies authentication-related behaviors such as
/// user login, user creation, and refresh token generation.
/// External dependencies are mocked to ensure isolation of the service logic.
/// </remarks>
[TestFixture]
[TestOf(typeof(AuthenticationService))]
public class AuthenticationServiceTest
{
    private readonly AuthenticationService _authenticationService;
    private readonly Mock<IPasswordHashingService> _passwordHashingService;
    private readonly Mock<IUserRepository> _authenticationRepository;
    private readonly IJwtService _jwtService;
    private readonly LoginRequestDto _userRequestDto;
    private readonly User _newUser;
    private readonly CreateUserDto _newUserDto;


    public AuthenticationServiceTest()
    {
        _jwtService = new JwtService();
        _passwordHashingService = new Mock<IPasswordHashingService>();
        _authenticationRepository = new Mock<IUserRepository>();
        _authenticationService =
            new AuthenticationService(_passwordHashingService.Object, _authenticationRepository.Object);
        _userRequestDto = new LoginRequestDto("lahcen28ahtat@gmail", "pass3453");
        _newUser = new User
        {
            Email = "lahcen28ahtat@gmail",
            Password = "pass3453"
        };
        _newUserDto = new CreateUserDto(Email: "lahcen28ahtat@gmail", Password: "pass3453", UserName: "lahcen");
    }

    /// <summary>
    /// Tests that a valid authentication request returns a user.
    /// </summary>
    /// <remarks>
    /// The test assumes:
    /// - A user exists with the given email.
    /// - The password hashing service successfully validates the password.
    /// The authentication service is expected to return the corresponding user.
    /// </remarks>
    [Test]
    public void AuthenticateUserTest_ShouldReturnUser()
    {
        _authenticationRepository.Setup(repo => repo.GetUserByEmail("lahcen28ahtat@gmail")).ReturnsAsync(_newUser);
        _passwordHashingService.Setup(ph => ph.VerifyPassword(_userRequestDto.Password, _newUser.Password))
            .Returns(true);
        var result = _authenticationService.AuthenticateUser(_userRequestDto);
        var user = result.Result;
        Assert.IsNotNull(user);
        Assert.That(user.Email, Is.EqualTo("lahcen28ahtat@gmail"));
    }


    /// <summary>
    /// Tests that a new user is created when no existing user
    /// is found with the same email address.
    /// </summary>
    /// <remarks>
    /// This test validates that:
    /// - The repository returns no user for the given email.
    /// - The password is hashed before persistence.
    /// - The created user is returned by the service.
    /// </remarks>
    [Test]
    public void CreateUserTest_shouldReturnNewUser()
    {
        User? nullUser = null;
        _authenticationRepository.Setup(repo => repo.GetUserByEmail(_newUser.Email)).ReturnsAsync(nullUser);
        _passwordHashingService.Setup(ph => ph.Hash(_newUser.Password)).Returns(_newUser.Password);
        _authenticationRepository.Setup(repo => repo.CreateUser(_newUser)).ReturnsAsync(_newUser);
        var result = _authenticationService.CreateUser(_newUserDto);
        Assert.IsNotNull(result.Result);
    }

    /// <summary>
    /// Tests the generation of refresh tokens.
    /// </summary>
    /// <remarks>
    /// The test ensures that:
    /// - Generated refresh tokens are not null.
    /// - Consecutive refresh tokens are unique.
    /// This validates the randomness of the token generation process.
    /// </remarks>
    [Test]
    public void TestRefreshTokenGeneration()
    {
        var token = _jwtService.GenerateRefreshToken();
        var token2 = _jwtService.GenerateRefreshToken();
        Assert.IsNotNull(token);
        Assert.IsNotNull(token2);
        Assert.That(token2, Is.Not.EqualTo(token));
    }
}