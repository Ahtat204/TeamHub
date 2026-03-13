using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using TeamCollaborationHub.server.IntegrationTest.TestDependencies;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Repositories.UserRepository;
using System.Text.Json;
using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using HttpRequestMessage = System.Net.Http.HttpRequestMessage;

namespace TeamCollaborationHub.server.IntegrationTest;

[ TestCaseOrderer("TeamCollaborationHub.server.IntegrationTest.TestDependencies.PriorityOrderer", "TeamCollaborationHub.server.IntegrationTest")]
public class IntegrationTest : BaseIntegrationTestFixture
{
    private readonly TeamHubApplicationFactory<Program, TdbContext> _applicationFactory;
    private readonly IUserRepository? _userRepository;
    private readonly IJwtService? _jwtService;
    private readonly User _user = new()
    {
        Name = "John Doe",
        Email = "test@test.com",
        Password = "password123",
    };
    private readonly User _userTest = new ()
    {
        Name = "Lahcen ahtat",
        Email = "lahce28ahtat@gmail.com",
        Password = "HiHI235417162",
    };

    public IntegrationTest(TeamHubApplicationFactory<Program, TdbContext> appFactory) : base(appFactory)
    {
        _applicationFactory = appFactory;
        _userRepository = scope.ServiceProvider.GetService<IUserRepository>();
        _jwtService = scope.ServiceProvider.GetService<IJwtService>();
    }

    #region DatabaseTests

    [Fact]
    public async Task InsertUserTest()
    {
        User createuserTest = new()
        {
            Name = "John Doe",
            Email = "test1@test.com",
            Password = "password123",
        };
        Assert.NotNull(_userRepository);
        var result = await _userRepository.CreateUser(createuserTest);
        Assert.NotNull(result);
        var response = await _userRepository.GetUserByEmail(result.Email);
        Assert.NotNull(response);
        Assert.Equal(createuserTest.Email, response.Email);
    }


    [Fact]
    public async Task DeleteUserTest()
    {
        Assert.NotNull(_userRepository);
        await _userRepository.CreateUser(_user);
        var response = await _userRepository.deleteUser(_user.Email);
        var getUserResponse = await _userRepository.GetUserByEmail(_user.Email);
        Assert.NotNull(response);
        Assert.Null(getUserResponse);
    }

    [Fact]
    public async Task RefreshTokenTestInsertionAndValidationTest()
    {
        Assert.NotNull(_userRepository);
        var user = await _userRepository.CreateUser(_userTest);
        Assert.NotNull(user);
        Assert.NotNull(_jwtService);
        var getUserResponse = await _userRepository.GetUserByEmail(user.Email);
        var refreshToken = new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
            Token = _jwtService.GenerateRefreshToken(),
        };
        var save = await _jwtService.SaveRefreshToken(refreshToken);
        var validToken = await _jwtService.ValidateRefreshToken(refreshToken.Token);
        var userToknen = await _jwtService.GetUserByRefreshToken(refreshToken.Id);

        #region nullCheck

        Assert.NotNull(user);
        Assert.NotNull(getUserResponse);
        Assert.NotNull(save);
        Assert.NotNull(validToken);
        Assert.NotNull(userToknen);

        #endregion

        #region EqualityCheck

        Assert.Equal(user.Id, userToknen.Id);
        Assert.Equal(refreshToken.Token, save);
        Assert.Equal(refreshToken.UserId, userToknen.Id);
        Assert.Equal(getUserResponse.Id, userToknen.Id);
        Assert.Equal(validToken.UserId, userToknen.Id);
        Assert.Equal(validToken.Token, refreshToken.Token);

        #endregion
    }

    #endregion

    #region AuthenticationEndpointsTests

    [Fact,TestPriority(1)]
    public async Task LoginTest() // successfully passed
    {
        LoginRequestDto request = new("lahcen30@gmail.com", "password123");
        string json = JsonSerializer.Serialize(request);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await Client.SendAsync(postRequest);
        var statusCode = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }


    [Fact]
    public async Task RegisterUserTest()
    {
        var userRegistrationRequest = new CreateUserDto("lahcen25@gmail.com", "123assword", "lahcen22");
        string registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json"),
        };
        var registerResponseMessage = await Client.SendAsync(registerPosHttpRequestMessage);
        //registerResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, registerResponseMessage.StatusCode);
    }

    [Fact]
    public async Task RegisterExistingUserTest_ShouldReturnBadRequest()
    {
        CreateUserDto? userRegistrationRequest = new CreateUserDto("lahcen26@gmail.com", "123assword", "lahcen22");
        string registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json"),
        };

        var registerResponseMessage = await Client.SendAsync(registerPosHttpRequestMessage);
        CreateUserDto? usersignUpRequest = new CreateUserDto("lahcen27@gmail.com", "123assword", "lahcen22");
        string secondRequest = JsonSerializer.Serialize(usersignUpRequest);
        var SecondRequest = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(secondRequest, Encoding.UTF8, "application/json")
        };
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var secondHttpRequest = await Client.SendAsync(SecondRequest);
        var refreshJson = await secondHttpRequest.Content.ReadAsStringAsync();
        var refreshResponse = JsonSerializer.Deserialize<RefreshAccessDto>(refreshJson, options);
        Assert.Null(refreshResponse?.RefreshToken);
        Assert.Null(refreshResponse?.AccessToken);
    }

    [Fact]
    public async Task LoginUserTest()
    {
        CreateUserDto? userRegistrationRequest = new CreateUserDto("lahcen21@gmail.com", "123password", "lahcen22");
        string registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json")
        };
        var registerResponseMessage = await Client.SendAsync(registerPosHttpRequestMessage);
        var UserRequest = new LoginRequestDto(userRegistrationRequest.Email, userRegistrationRequest.Password);
        string json = JsonSerializer.Serialize(UserRequest);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await Client.SendAsync(postRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.NotEmpty(response.Content.ToString());
    }


    [Fact]
    public async Task RefreshTokenTest()
    {
        CreateUserDto? userRegistrationRequest = new CreateUserDto("lahcen20@gmail.com", "123assword", "lahcen22");
        string registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json")
        };
        var registerResponseMessage = await Client.SendAsync(registerPosHttpRequestMessage);
        var userRequest = new LoginRequestDto(userRegistrationRequest.Email, userRegistrationRequest.Password);
        string json = JsonSerializer.Serialize(userRequest);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await Client.SendAsync(postRequest);
        response.EnsureSuccessStatusCode();

        #region HappyPath

        string jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<LoginResponseDto>(jsonString, options);
        if(result?.RefreshToken is null) return ;
        var refreshTokenRequest = new RefreshToken()
        {
            Token = result.RefreshToken.Token,
            Id = Guid.TryParse(result.RefreshToken.Id, out Guid token) ? token : Guid.Empty,
        };
        string jsonToken = JsonSerializer.Serialize(refreshTokenRequest);
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/refresh")
        {
            Content = new StringContent(jsonToken, Encoding.UTF8, "application/json")
        };
        var refreshResponseMessage = await Client.SendAsync(requestMessage);
        refreshResponseMessage.EnsureSuccessStatusCode();
        var refreshJson = await refreshResponseMessage.Content.ReadAsStringAsync();
        var refreshResponse = JsonSerializer.Deserialize<RefreshAccessDto>(refreshJson, options);
        #endregion
        #region Assertions
        Assert.NotNull(refreshResponse);
        Assert.NotNull(refreshResponse.RefreshToken);
        Assert.NotNull(refreshResponse.AccessToken);
        Assert.Equal(HttpStatusCode.OK, refreshResponseMessage.StatusCode);
        Assert.NotNull(result);
        #endregion
    }

    [Fact,TestPriority(2)]
    public async Task RateLimitTest()
    {
        LoginRequestDto request = new("lahcen30@gmail.com", "password123");
        string json = JsonSerializer.Serialize(request);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await Client.SendAsync(postRequest);
        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    #endregion
}