using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using Moq;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Security;

namespace TeamcollaborationHub.server.UnitTest.Services.Authentication.UserAuthentication;

[TestFixture]
[TestOf(typeof(AuthenticationService))]
public class AuthenticationServiceTest
{
    private readonly AuthenticationService _authenticationService;
    private readonly Mock<IPasswordHashingService> _passwordHashingService;
    private readonly Mock<IUserRepository> _authenticationRepository;
    private readonly UserRequestDto _userRequestDto;
    private readonly User _newUser;
    private readonly CreateUserDto _newUserDto;

    public AuthenticationServiceTest()
    {
        _passwordHashingService = new Mock<IPasswordHashingService>();
        _authenticationRepository = new Mock<IUserRepository>();
        _authenticationService =
            new AuthenticationService(_passwordHashingService.Object, _authenticationRepository.Object);
        _userRequestDto = new UserRequestDto("lahcen28ahtat@gmail", "pass3453");
        _newUser = new User
        {
            Email = "lahcen28ahtat@gmail",
            Password = "pass3453"
        };
        _newUserDto = new CreateUserDto(Email: "lahcen28ahtat@gmail",Password: "pass3453", UserName: "lahcen");
    }

    [Test]
    public void AuthenticateUserTest_ShouldReturnUser()
    {
        _authenticationRepository.Setup(repo => repo.GetUserByEmail("lahcen28ahtat@gmail")).ReturnsAsync(_newUser);
        _passwordHashingService.Setup(ph => ph.VerifyPassword(_userRequestDto.Password, _newUser.Password)).Returns(true);
        var result = _authenticationService.AuthenticateUser(_userRequestDto);
        var user = result.Result;
        Assert.IsNotNull(user);
        Assert.That(user.Email, Is.EqualTo("lahcen28ahtat@gmail"));
    }

    [Test]
    public void CreateUserTest_shouldReturnNewUser()
    {
        
        _authenticationRepository.Setup(repo => repo.GetUserByEmail(_newUser.Email)).ReturnsAsync(null as User);
        _passwordHashingService.Setup(ph => ph.Hash(_newUser.Password)).Returns(_newUserDto.Password);
        _authenticationRepository.Setup(repo => repo.CreateUser(_newUser)).ReturnsAsync(_newUser);
        var result = _authenticationService.CreateUser(_newUserDto);
        var user = result.Result;
        Assert.IsNotNull(user);
        Assert.That(user.Email, Is.EqualTo("lahcen28ahtat@gmail"));
    }
}