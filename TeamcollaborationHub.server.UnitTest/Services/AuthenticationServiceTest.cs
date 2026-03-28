using Moq;
using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Security;

namespace TeamcollaborationHub.server.UnitTest.Services;

[TestFixture]
[TestOf(typeof(AuthenticationService))]
public class AuthenticationServiceTest
{
    private readonly AuthenticationService _authenticationService;
    private readonly Mock<IPasswordHashingService> _passwordHashingService;
    private readonly Mock<IUserRepository> _authenticationRepository;
    private readonly ITokenService _tokenService;
    private readonly LoginRequestDto _userRequestDto;
    private readonly User _newUser;
    private readonly CreateUserDto _newUserDto;


    public AuthenticationServiceTest()
    {
        _tokenService = new TokenService();
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
    public async Task AuthenticateUserTest_ShouldReturnUser()
    {
        _authenticationRepository.Setup(repo => repo.GetUserByEmail("lahcen28ahtat@gmail")).ReturnsAsync(_newUser);
        _passwordHashingService.Setup(ph => ph.VerifyPassword(_userRequestDto.Password, _newUser.Password))
            .Returns(true);
        var user = await _authenticationService.AuthenticateUser(_userRequestDto);
        Assert.IsNotNull(user);
        Assert.That(user.Email, Is.EqualTo("lahcen28ahtat@gmail"));
    }

    [Test]
    public void AuthenticateUserTest_ShouldThrowNotFoundException()
    {
        User? nullUser = null;
        _authenticationRepository.Setup(repo => repo.GetUserByEmail("lahcen28ahtat@gmail")).ReturnsAsync(nullUser);
        Assert.That(() => _authenticationService.AuthenticateUser(_userRequestDto),
            Throws.Exception.TypeOf<NotFoundException<User>>());
    }

    [Test]
    public void AuthenticateUserTest_ShouldThrowException()
    {
        var wrongUser = new User()
        {
            Email = "lahcen28ahtat@gmail",
            Password = "pass345345171NSYU2"
        };
        _authenticationRepository.Setup(repo => repo.GetUserByEmail("lahcen28ahtat@gmail")).ReturnsAsync(wrongUser);
        _passwordHashingService.Setup(ph => ph.VerifyPassword(_userRequestDto.Password, wrongUser.Password))
            .Returns(false);
        Assert.That(() => _authenticationService.AuthenticateUser(_userRequestDto),
            Throws.Exception.TypeOf<NotFoundException<User>>());
    }

    [Test]
    public async Task CreateUserTest_shouldReturnNewUser()
    {
        User? nullUser = null;
        _authenticationRepository.Setup(repo => repo.GetUserByEmail(_newUser.Email)).ReturnsAsync(nullUser);
        _passwordHashingService.Setup(ph => ph.Hash(_newUser.Password)).Returns(_newUser.Password);
        _authenticationRepository.Setup(repo => repo.CreateUser(_newUser)).ReturnsAsync(_newUser);
        var result = await _authenticationService.CreateUser(_newUserDto);
        Assert.IsNotNull(result);
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
        var token = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();
        Assert.IsNotNull(token);
        Assert.IsNotNull(token2);
        Assert.That(token2, Is.Not.EqualTo(token));
    }

    [Test]
    public async Task UpdateUserTest()
    {
        var updatedUser = _newUser;
        updatedUser.Password = "pas9826bsya&ndh2245mlnuo";
        _authenticationRepository.Setup(repo => repo.GetUserByEmail(_newUser.Email)).ReturnsAsync(_newUser);
        _passwordHashingService.Setup(ph => ph.Hash(_newUser.Password)).Returns("pas9826bsya&ndh2245mlnuo");
        _authenticationRepository.Setup(repo => repo.UpdateUser(_newUser)).ReturnsAsync(updatedUser);
        var result =
            await _authenticationService.UpdatePassword(new UpdatePasswordDto(_newUser.Email, updatedUser.Password));
        Assert.IsNotNull(result);
        Assert.That(result.Email, Is.EqualTo(_newUser.Email));
    }

    [Test]
    public void UpdatePasswordTest_ShouldThrowNotFoundException()
    {
        _authenticationRepository.Setup(repo => repo.GetUserByEmail(_newUser.Email)).ReturnsAsync((User)null);
        Assert.That(()=>_authenticationService.UpdatePassword(new UpdatePasswordDto(_newUser.Email,"pas9826bsya")),Throws.Exception.TypeOf<NotFoundException<User>>());
    }
}