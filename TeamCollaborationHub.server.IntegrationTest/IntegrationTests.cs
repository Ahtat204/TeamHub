using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamCollaborationHub.server.IntegrationTest.TestDependencies;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Features.Projects.Commands.CreateProject;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.Jwt;


namespace TeamCollaborationHub.server.IntegrationTest;

[TestCaseOrderer("TeamCollaborationHub.server.IntegrationTest.TestDependencies.PriorityOrderer",
    "TeamCollaborationHub.server.IntegrationTest")]
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

    private readonly User _userTest = new()
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

    #region UserTableTests

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

    #region ProjectTableTests

    [Fact]
    public async Task CreateProject()
    {
        Project project = new()
        {
            Name = "Project 1",
            Description = "namedeed",
        };
        Assert.NotNull(project);
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        var result = await context.Projects.FirstOrDefaultAsync();
        Assert.NotNull(result);
        project.Id = result.Id;
        Assert.Equal(project.Name, result.Name);
    }


    [Fact]
    public async Task AddContributorToProject()
    {
        Project project = new()
        {
            Name = "TeamHub",
            Description = "ASP.NET Core Web API",
        };
        Assert.NotNull(project);
        Assert.NotNull(project);
        context.Projects.Add(project);
        User contributor = new User()
        {
            Name = "JlalalaDoe",
            Email = "ahtat203@test.com",
            Password = "password123",
            project = project
        };
        await context.AddAsync(contributor);
        await context.SaveChangesAsync();
        var updatedProject = context.Projects.Include(pr => pr.Contributors).FirstOrDefault(pr => pr.Id == project.Id);
        await context.SaveChangesAsync();
        Assert.NotNull(updatedProject);
        Assert.NotNull(updatedProject.Contributors);
        Assert.NotEmpty(updatedProject.Contributors);
        Assert.Equal(contributor.Email, updatedProject.Contributors.First().Email);
    }

    [Fact]
    public async Task RemoveContributorFromProject()
    {
        Project project = new()
        {
            Name = "ShootTrainner",
            Description = "Unreal Engine C++ Game",
        };
        Assert.NotNull(project);
        Assert.NotNull(project);
        context.Projects.Add(project);
        User contributor = new User()
        {
            Name = "JlalalaDoe",
            Email = "lahcen203@test.com",
            Password = "password123",
            project = project
        };
        await context.AddAsync(contributor);
        contributor.project = null;
        contributor.ProjectId = null;
        context.Users.Add(contributor);
        await context.SaveChangesAsync();
        await context.SaveChangesAsync();
        var updatedProject = context.Users.FirstOrDefault(u => u.project == project);
        Assert.Null(updatedProject);
    }

    [Fact]
    public async Task AddProjectTask()
    {
        Project pro = new()
        {
            Deadline = DateTime.Today,
            Name = "Solar System",
            Description = "do I know you?,if no,this is a Physics simulation(not this, the C++ repo)",
            Status = ProjectStatus.InProgress
        };
        ProjectTask projectTask = new()
        {
            project = pro,
            Title = "nothing , just a Metric Tensor(the Riemann's)",
            Description = "do I know you",
            projectId = pro.Id,
        };
        await context.Projects.AddAsync(pro);
        await context.Tasks.AddAsync(projectTask);
        await context.SaveChangesAsync();
        var fetchedtask = context.Tasks.FirstOrDefault(t => t.project == pro);
        Assert.NotNull(fetchedtask);
        Assert.Equal(fetchedtask.Title, projectTask.Title);
    }

    [Fact]
    public async Task RemoveProjectTask()
    {
        Project pro = new()
        {
            Deadline = DateTime.Today,
            Name = "Physics Informed Neural Networks",
            Description = "the Intersection of DL and Partial Differential Equation",
            Status = ProjectStatus.Started
        };
        ProjectTask projectTask = new()
        {
            project = pro,
            Title = "I'm not a python addict,if I'd implement PINNs,I'd use LibTorch",
            Description = "do I know you",
            projectId = pro.Id,
        };
        await context.Projects.AddAsync(pro);
        context.SaveChanges();
        var taskResult = context.Tasks.AddAsync(projectTask).Result.Entity;
        context.SaveChanges();
        context.Tasks.Remove(taskResult);
        await context.SaveChangesAsync();
        var fetchedtask = context.Tasks.FirstOrDefault(t => t.Id == taskResult.Id);
        Assert.Null(fetchedtask);
    }

    #endregion

    #region AuthenticationEndpointsTests

    [Fact, TestPriority(0)]
    public async Task LoginTest()
    {
        LoginRequestDto request = new("lahcen30@gmail.com", "password123");
        string json = JsonSerializer.Serialize(request);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await Client.SendAsync(postRequest);
        await response.Content.ReadAsStringAsync();
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
        registerResponseMessage.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, registerResponseMessage.StatusCode);
    }

    [Fact]
    public async Task RegisterExistingUserTest_ShouldReturnBadRequest()
    {
        var userRegistrationRequest = new CreateUserDto("lahcen26@gmail.com", "123assword", "lahcen22");
        var registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json"),
        };

        await Client.SendAsync(registerPosHttpRequestMessage);
        var userSignUpRequest = new CreateUserDto("lahcen27@gmail.com", "123assword", "lahcen22");
        string secondRequest = JsonSerializer.Serialize(userSignUpRequest);
        var secondResponse = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(secondRequest, Encoding.UTF8, "application/json")
        };
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var secondHttpRequest = await Client.SendAsync(secondResponse);
        var refreshJson = await secondHttpRequest.Content.ReadAsStringAsync();
        var refreshResponse = JsonSerializer.Deserialize<RefreshAccessDto>(refreshJson, options);
        Assert.Null(refreshResponse?.RefreshToken);
        Assert.Null(refreshResponse?.AccessToken);
    }

    [Fact]
    public async Task LoginUserTest()
    {
        var userRegistrationRequest = new CreateUserDto("lahcen21@gmail.com", "123password", "lahcen22");
        var registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json")
        };
        await Client.SendAsync(registerPosHttpRequestMessage);
        var userRequest = new LoginRequestDto(userRegistrationRequest.Email, userRegistrationRequest.Password);
        var json = JsonSerializer.Serialize(userRequest);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await Client.SendAsync(postRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(body);
    }


    [Fact]
    public async Task RefreshTokenTest()
    {
        var userRegistrationRequest = new CreateUserDto("lahcen20@gmail.com", "123assword", "lahcen22");
        var registerJson = JsonSerializer.Serialize(userRegistrationRequest);
        var registerPosHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/signup")
        {
            Content = new StringContent(registerJson, Encoding.UTF8, "application/json")
        };
        await Client.SendAsync(registerPosHttpRequestMessage);
        var userRequest = new LoginRequestDto(userRegistrationRequest.Email, userRegistrationRequest.Password);
        var json = JsonSerializer.Serialize(userRequest);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await Client.SendAsync(postRequest);
        response.EnsureSuccessStatusCode();

        #region HappyPath

        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<LoginResponseDto>(jsonString, options);
        if (result?.RefreshToken is null) return;
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

    [Fact, TestPriority(5)]
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

    #region ProjectEndpointsTests

    [Fact, TestPriority(4)]
    public async Task GetProjectsTest()
    {
        var postRequest = new HttpRequestMessage(HttpMethod.Get, "/api/projects");
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact, TestPriority(3)]
    public async Task CreateProjects()
    {
        CreateProjectCommand request = new CreateProjectCommand(Name: "Pro22", Contributors: new Collection<User>()
            {
                new User
                {
                    Name = "John Doe",
                    Email = "test@test.com",
                    Password = "password123"
                },
                new User
                {
                    Name = "John Doe",
                    Email = "nottesting@test.com",
                    Password = "password123"
                }
            },
            Description: "namedeed",
            ProjectStatus: ProjectStatus.Started,
            Deadline: DateTime.Today);

        string json = JsonSerializer.Serialize(request);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/projects")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact, TestPriority(3)]
    public async Task GetProjectByIdTest()
    {
        var prorandom = context.Projects.FirstOrDefault();
        Assert.NotNull(prorandom);
        var postRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/projects/{prorandom.Id}");
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact, TestPriority(2)]
    public async Task GetAllProjectContributorsTest()
    {
        var prorandom = context.Projects.FirstOrDefault();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        User contributor = new ()
        {
            Name = "JlalalaDoe",
            Email = "atat203@test.com",
            Password = "password123",
            project = prorandom
        };
        await context.AddAsync(contributor);
        await context.SaveChangesAsync();
        var postRequest = new HttpRequestMessage(HttpMethod.Get, $"api/projects/{prorandom.Id}/contributors");
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<IEnumerable<User>>(jsonString, options);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    #endregion
}