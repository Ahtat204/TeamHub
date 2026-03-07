using Moq;
using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Security;

namespace TeamcollaborationHub.server.UnitTest.Services.Authentication.UserAuthentication;

[TestFixture]
[TestOf(typeof(AuthenticationService))]
public class AuthenticationServiceTest
{

    private Mock<IPasswordHashingService> _passwordHashingMock;
    private Mock<IUserRepository> _authRepoMock;
    private Mock<IJwtService> _jwtServiceMock;
    private AuthenticationService _authService;

    [SetUp]
    public void Setup()
    {
        _passwordHashingMock = new Mock<IPasswordHashingService>();
        _authRepoMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();

        _authService = new AuthenticationService(
            _passwordHashingMock.Object,
            _authRepoMock.Object,
            _jwtServiceMock.Object
        );
    }
    [Test]
    public async Task AuthenticateUser_Success_ReturnsAuthenticationResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var user = new User { Email = email, Password = "hashedPassword", Name = "Test User" };
        var token = "token";
        var expiry = DateTime.UtcNow.AddHours(1);
        int expiryDate = (int)expiry.Subtract(DateTime.UnixEpoch).TotalSeconds;

        _authRepoMock.Setup(r => r.GetUserByEmail(email)).ReturnsAsync(user);
        _passwordHashingMock.Setup(h => h.VerifyPassword(password, user.Password)).Returns(true);
        _jwtServiceMock.Setup(j => j.GenerateTokenResponse(user, out expiryDate)).Returns(token);

        var requestDto = new UserRequestDto(Email: email, Password: password);

        // Act
        var result = await _authService.AuthenticateUser(requestDto);

        // Assert
        Assert.NotNull(result);
        Assert.That(result.email, Is.EqualTo(email));
        Assert.That(result.AccessToken, Is.EqualTo(token));
    }
}