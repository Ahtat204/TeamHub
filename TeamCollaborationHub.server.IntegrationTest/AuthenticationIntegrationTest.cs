using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TeamCollaborationHub.server.IntegrationTest.TestDependencies;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Repositories.UserRepository;
using System.Text.Json;
using TeamcollborationHub.server.Services.Authentication.Jwt;

namespace TeamCollaborationHub.server.IntegrationTest;

public class AuthenticationIntegrationTest : BaseIntegrationTestFixture
{
    private readonly TeamHubApplicationFactory<Program, TdbContext> _applicationFactory;
    private readonly IUserRepository? _userRepository;
    private readonly IJwtService? _jwtService;

    private User user = new()
    {
        Name = "John Doe",
        Email = "test@test.com",
        Password = "password123",
    };

    private User usertest = new User()
    {
        Name = "Lahcen ahtat",
        Email = "lahce28ahtat@gmail.com",
        Password = "HiHI235417162",
    };

    public AuthenticationIntegrationTest(TeamHubApplicationFactory<Program, TdbContext> appFactory) : base(appFactory)
    {
        _applicationFactory = appFactory;
        _userRepository = scope.ServiceProvider.GetService<IUserRepository>();
        _jwtService = scope.ServiceProvider.GetService<IJwtService>();
    }

    #region DatabaseTests

    [Fact]
    public async Task InsertUserTest()
    {
        var result = await _userRepository?.CreateUser(user);
        var response = await _userRepository?.GetUserByEmail(result?.Email);
        Assert.NotNull(response);
        Assert.Equal(user.Email, response?.Email);
    }


    [Fact]
    public async Task DeleteUserTest()
    {
        var request = await _userRepository?.CreateUser(user);
        var response = await _userRepository?.deleteUser(user.Email);
        var getUserResponse = await _userRepository?.GetUserByEmail(user.Email);
        Assert.NotNull(response);
        Assert.Null(getUserResponse);
    }

    [Fact]
    public async Task RefreshTokenTestInsertionAndValidationTest()
    {
        var user = await _userRepository?.CreateUser(usertest);
        var getUserResponse = await _userRepository?.GetUserByEmail(user.Email);
        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
            Token = _jwtService.GenerateRefreshToken(),
        };
        var save=await _jwtService.SaveRefreshToken(refreshToken);
        var validToken=await _jwtService.ValidateRefreshToken(refreshToken.Token);
        var userToknen=await _jwtService.GetUserByRefreshToken(refreshToken.Id);
        #region nullCheck
        Assert.NotNull(user);
        Assert.NotNull(getUserResponse);
        Assert.NotNull(save);
        Assert.NotNull(validToken);
        Assert.NotNull(userToknen);
        #endregion
        #region EqualityCheck
        Assert.Equal(user.Id, userToknen.Id);
        Assert.Equal(refreshToken.Token,save);
        Assert.Equal(refreshToken.UserId, userToknen.Id);
        Assert.Equal(getUserResponse.Id, userToknen.Id);
        Assert.Equal(validToken.UserId, userToknen.Id);
        Assert.Equal(validToken.Token,refreshToken.Token);
        
        #endregion
    }
    #endregion

    #region AuthenticationEndpointsTests

    [Fact]
    public async Task LoginTest() // successfully passed
    {
        using var httpClient = _applicationFactory.CreateClient();
        LoginRequestDto? request = new("lahcen30@gmail.com", "password123");
        string json = JsonSerializer.Serialize(request);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(postRequest);
        var statusCode = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task RegisterUserTest()
    {
        using var httpClient = _applicationFactory.CreateClient();
        CreateUserDto? userRegistrationRequest = new CreateUserDto("lahcen25@gmail.com", "123assword", "lahcen22");
        string registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json")
        };
        var registerResponseMessage = await httpClient.SendAsync(registerPosHttpRequestMessage);
        registerResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, registerResponseMessage.StatusCode);
    }

    [Fact]
    public async Task LoginUserTest()
    {
        using var httpClient = _applicationFactory.CreateClient();
        CreateUserDto? userRegistrationRequest = new CreateUserDto("lahcen28@gmail.com", "123password", "lahcen22");
        string registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json")
        };
        var registerResponseMessage = await httpClient.SendAsync(registerPosHttpRequestMessage);
        var UserRequest = new LoginRequestDto(userRegistrationRequest.Email, userRegistrationRequest.Password);
        string json = JsonSerializer.Serialize(UserRequest);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await httpClient.SendAsync(postRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content.ToString());
        Assert.NotEmpty(response.Content.ToString());
    }


    [Fact]
    public async Task RefreshTokenTest()
    {
    }

    #endregion
}