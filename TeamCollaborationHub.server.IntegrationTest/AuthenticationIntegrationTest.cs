using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TeamCollaborationHub.server.IntegrationTest.TestDependencies;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories.UserRepository;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TeamCollaborationHub.server.IntegrationTest;

public class AuthenticationIntegrationTest : BaseIntegrationTestFixture
{
    private readonly TeamHubApplicationFactory<Program, TDBContext> _applicationFactory;
    private readonly IUserRepository? _userRepository;

    private User user = new()
    {
        Name = "John Doe",
        Email = "test@test.com",
        Password = "password123",
    };

    public AuthenticationIntegrationTest(TeamHubApplicationFactory<Program, TDBContext> appFactory) : base(appFactory)
    {
        _applicationFactory = appFactory;
        _userRepository = scope.ServiceProvider.GetService<IUserRepository>();
    }

    #region DatabaseTests

    [Fact]
    public async Task InsertUserTest()
    {
        var result = await _userRepository?.CreateUser(user);
        var response = await  _userRepository?.GetUserByEmail(result?.Email);
        Assert.NotNull(response);
        Assert.Equal(user.Email, response?.Email);
    }


    [Fact]
    public async Task DeleteUserTest()
    {
        var request = await  _userRepository?.CreateUser(user);
        var response = await  _userRepository?.deleteUser(user.Email);
        var getUserResponse = await  _userRepository?.GetUserByEmail(user.Email);
        Assert.NotNull(response);
        Assert.Null(getUserResponse);
    }

    #endregion

    #region AuthenticationTests


    [Fact]
    public async Task LoginTest() // successfully passed
    {
        using var httpClient = _applicationFactory.CreateClient();
        UserRequestDto? request = new ("lahcen28@gmail.com", "123password");
        string json=JsonSerializer.Serialize(request);
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
        CreateUserDto? userRegistrationRequest=new CreateUserDto("lahcen28@gmail.com", "123password","lahcen22");
        string registerJson=JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json")
        };
        var registerResponseMessage = await httpClient.SendAsync(registerPosHttpRequestMessage);
        registerResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, registerResponseMessage.StatusCode);
    }

    #endregion
}