using Microsoft.AspNetCore.Mvc;
using Moq;
using TeamcollborationHub.server.Controllers;
using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;

namespace TeamcollaborationHub.server.UnitTest.Controllers;

[TestFixture]
[TestOf(typeof(AuthenticationController))]
public class AuthenticationControllerTest
{
    private readonly Mock<IAuthenticationService> _authenticationService;
    private readonly Mock<ITokenService> _jwtService;
    private readonly AuthenticationController _controller;
    private readonly User _newUser;
    private const string Secret = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Ikpv";

    public AuthenticationControllerTest()
    {
        _authenticationService = new Mock<IAuthenticationService>();
        _jwtService = new Mock<ITokenService>();
        _controller = new AuthenticationController(_authenticationService.Object, _jwtService.Object);
        _newUser = new User
        {
            Email = "lahcen28ahtat@gmail",
            Password = "pass3453"
        };
    }


    [Test]
    public async Task LogintestShouldreturnOk()
    {
        _authenticationService.Setup(service =>
            service.AuthenticateUser(new LoginRequestDto(_newUser.Email, _newUser.Password))).ReturnsAsync(_newUser);
        int tokenExpiryDate;
        _jwtService.Setup(jwts => jwts.GenerateTokenResponse(_newUser,out tokenExpiryDate)).Returns(Secret);
        var result =await _controller.Login(new LoginRequestDto(_newUser.Email, _newUser.Password));
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ActionResult<LoginResponseDto>>());
    }

    [Test]
    public async Task LogintestShouldreturnBadRequest()
    {
        User? nullUser = null;
        _authenticationService.Setup(service =>
            service.AuthenticateUser(new LoginRequestDto(_newUser.Email, _newUser.Password))).ReturnsAsync(nullUser);
        int tokenExpiryDate;
        _jwtService.Setup(jwts => jwts.GenerateTokenResponse(_newUser,out tokenExpiryDate)).Returns(Secret);
        var result = await _controller.Login(new LoginRequestDto(_newUser.Email, _newUser.Password));
        Assert.NotNull(result.Result);
        var response = result.Result;
        Assert.That(response, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task LogintestShouldreturnbadrequest()
    {
        User? nullUser = null;
        LoginRequestDto? nullInput = null;
        _authenticationService.Setup(service =>
            service.AuthenticateUser(nullInput)).ReturnsAsync(nullUser);
        int tokenExpiryDate;
        _jwtService.Setup(jwt => jwt.GenerateTokenResponse(_newUser,out tokenExpiryDate)).Returns(Secret);
        var result = await _controller.Login(nullInput);
        Assert.NotNull(result.Result);
        var response = result.Result;
        Assert.That(response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task SignUpTestShouldreturnOk()
    {
        CreateUserDto userRegistrationRequest = new ("lahcen25@gmail.com", "123assword", "lahcen22");
        User? result = new()
        {
            Email = "lahcen25@gmail.com",
            Password = "pass3453",
            Name = userRegistrationRequest.UserName
        };
        _authenticationService.Setup(service =>
            service.CreateUser(userRegistrationRequest)).ReturnsAsync(result);
        var response = await _controller.SignUp(userRegistrationRequest);
        Assert.That(response, Is.Not.Null);
        var type = response.Result;
        Assert.Multiple(() =>
        {
            Assert.That(type, Is.InstanceOf<OkObjectResult>());
        });
        
    }
}