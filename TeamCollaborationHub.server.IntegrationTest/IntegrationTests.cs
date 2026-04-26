using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeamCollaborationHub.server.IntegrationTest.TestDependencies;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Dto;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;
using TeamcollborationHub.server.Features.Projects.Commands.AddProjectComment;
using TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;
using TeamcollborationHub.server.Features.Projects.Commands.CreateProject;
using TeamcollborationHub.server.Features.Projects.Commands.SetProjectDeadline;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Caching;

namespace TeamCollaborationHub.server.IntegrationTest;

/// <summary>
/// Provides comprehensive Integration Tests for the TeamCollaborationHub API.
/// </summary>
/// <remarks>
/// This class orchestrates end-to-end testing by spinning up a test server using 
/// <see cref="TeamHubApplicationFactory{TEntryPoint, TDbContext}"/>. It verifies the 
/// full request-response lifecycle, including database persistence, Redis caching logic, 
/// and the Middleware pipeline (Authentication/Rate Limiting).
/// </remarks>
[TestCaseOrderer("TeamCollaborationHub.server.IntegrationTest.TestDependencies.PriorityOrderer",
    "TeamCollaborationHub.server.IntegrationTest")]
public class IntegrationTest : BaseIntegrationTestFixture
{
    /// <summary>
    /// Initialized via Constructor Injection to manage the lifecycle of the TestHost.
    /// </summary>
    private readonly TeamHubApplicationFactory<Program, TdbContext> _applicationFactory;

    /// <summary>
    /// Repository to verify data integrity directly in the underlying SQL provider.
    /// </summary>
    private readonly IUserRepository? _userRepository;

    /// <summary>
    /// Service used to simulate valid JWT generation for authorized endpoint testing.
    /// </summary>
    private readonly ITokenService? _jwtService;
    
    private readonly ICachingService<Project, string>? _cachingService;

    /// <summary>
    /// Stores a persistent Bearer Token used across authorized request test cases.
    /// </summary>
    private string? token;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTest"/> class.
    /// </summary>
    /// <param name="appFactory">The factory used to create the <see cref="TeamCollaborationHub.server.IntegrationTest.TestDependencies.TeamHubApplicationFactory"/>.</param>
    public IntegrationTest(TeamHubApplicationFactory<Program, TdbContext> appFactory) : base(appFactory)
    {
        _applicationFactory = appFactory;
        _userRepository = scope.ServiceProvider.GetService<IUserRepository>();
        _jwtService = scope.ServiceProvider.GetService<ITokenService>();
        token = _jwtService?.GenerateTokenResponse(_user, out var date);
        _cachingService = scope.ServiceProvider.GetService<ICachingService<Project, string>>();
    }

    #region UserTableTests

    /// <summary>
    /// Verifies that a user can be successfully persisted and retrieved from the User repository.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
        var response = await _userRepository.DeleteUser(_user.Email);
        var getUserResponse = await _userRepository.GetUserByEmail(_user.Email);
        Assert.NotNull(response);
        Assert.Null(getUserResponse);
    }

    /// <summary>
    /// Tests the full lifecycle of a Refresh Token, ensuring it is correctly 
    /// mapped to a User and validates against expiration logic.
    /// </summary>
    /// <remarks>
    /// This test confirms the security integrity of the <see cref="ITokenService"/> implementation.
    /// </remarks>
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

    /// <summary>
    /// Creates a new project and verifies persistence in the database.
    /// </summary>
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

    /// <summary>
    /// Removes a contributor from a project and verifies disassociation.
    /// </summary>
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

    /// <summary>
    /// Adds a task to a project and validates persistence.
    /// </summary>
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

    /// <summary>
    /// Removes a task from a project and ensures deletion.
    /// </summary>
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
        await context.SaveChangesAsync();
        var taskResult = context.Tasks.AddAsync(projectTask).Result.Entity;
        await context.SaveChangesAsync();
        context.Tasks.Remove(taskResult);
        await context.SaveChangesAsync();
        var fetchedtask = context.Tasks.FirstOrDefault(t => t.Id == taskResult.Id);
        Assert.Null(fetchedtask);
    }

    #endregion

    #region AuthenticationEndpointsTests

    /// <summary>
    /// Attempts login with invalid credentials and expects a NotFound response.
    /// </summary>
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

    /// <summary>
    /// Registers a new user and expects a successful response.
    /// </summary>
    [Fact, TestPriority(1)]
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

    /// <summary>
    /// Ensures that the <c>/signup</c> endpoint prevents duplicate registration and 
    /// returns a null payload for existing entities.
    /// </summary>
    [Fact, TestPriority(2)]
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

    /// <summary>
    /// Registers and logs in a user, verifying access token issuance.
    /// </summary>
    [Fact, TestPriority(3)]
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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var refreshResponse = JsonSerializer.Deserialize<LoginResponseDto>(body, options);
        Assert.NotNull(refreshResponse);
        Assert.NotNull(refreshResponse.AccessToken);
    }

    /// <summary>
    /// Validates refresh token exchange for new access tokens.
    /// </summary>
    [Fact, TestPriority(4)]
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

    #endregion

    #region ProjectEndpointsTests

    #region GetRequests

    /// <summary>
    /// Retrieves all projects via API and expects a successful response.
    /// </summary>
    [Fact, TestPriority(5)]
    public async Task GetAllProjectsTest()
    {
        var postRequest = new HttpRequestMessage(HttpMethod.Get, "/api/projects")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Retrieves a project by ID and expects a successful response.
    /// </summary>
    [Fact, TestPriority(6)]
    public async Task GetProjectByIdTest()
    {
        var prorandom = context.Projects.FirstOrDefault();
        Assert.NotNull(prorandom);
        var getrequest = new HttpRequestMessage(HttpMethod.Get, $"/api/projects/{prorandom.Id}")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(getrequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Retrieves contributors for a project and ensures non-empty results.
    /// </summary>
    [Fact, TestPriority(7)]
    public async Task GetAllProjectContributorsTest()
    {
        var prorandom = context.Projects.FirstOrDefault();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        User contributor = new()
        {
            Name = "JlalalaDoe",
            Email = "atat203@test.com",
            Password = "password123",
            project = prorandom
        };
        await context.AddAsync(contributor);
        await context.SaveChangesAsync();
        var getrequest = new HttpRequestMessage(HttpMethod.Get, $"api/projects/{prorandom.Id}/contributors")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(getrequest);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<IEnumerable<User>>(jsonString, options);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    /// <summary>
    /// Retrieves tasks for a project and ensures non-empty results.
    /// </summary>
    [Fact, TestPriority(8)]
    public async Task GetAllProjectTasks()
    {
        var prorandom = context.Projects.FirstOrDefault();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        ProjectTask newTask = new()
        {
            Title = "LEqualsTPlusV",
            Description = "LagrangianIsEqualToKineticEnergyPlusPotentialEnergy",
            projectId = prorandom.Id,
        };
        await context.AddAsync(newTask);
        await context.SaveChangesAsync();
        var getrequest = new HttpRequestMessage(HttpMethod.Get, $"api/projects/{prorandom.Id}/tasks")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(getrequest);
        Assert.NotNull(response);
        Assert.NotNull(response.Content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<IEnumerable<ProjectTask>>(jsonString, options);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    /// <summary>
    /// Retrieves a project task by ID and validates correctness.
    /// </summary>
    [Fact, TestPriority(9)]
    public async Task GetProjectTaskByIdTest()
    {
        var prorandom = context.Projects.Include(project => project.Tasks).FirstOrDefault();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        Assert.NotNull(prorandom.Tasks);
        var task = prorandom.Tasks.FirstOrDefault();
        Assert.NotNull(task);
        var getrequest = new HttpRequestMessage(HttpMethod.Get, $"api/project/tasks/{task.Id}")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(getrequest);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ProjectTask>(jsonString, options);
        Assert.NotNull(result);
    }

    #endregion

    #region PostRequests

    /// <summary>
    /// Creates a new project and verifies persistence in the database.
    /// </summary>
    [Fact, TestPriority(10)]
    public async Task CreateProjects()
    {
        var request = new CreateProjectCommand(Name: "Pro22", Contributors: new Collection<User>()
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
        Assert.NotNull(token);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "api/projects")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
       
    }

    /// <summary>
    /// Adds a contributor to a project via API and validates persistence.
    /// </summary>
    [Fact, TestPriority(11)]
    public async Task AddContributorToProjectTest()
    {
        var prorandom = context.Projects.FirstOrDefault();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        User contributor = new()
        {
            Email = "len28ahtat@gmail",
            Password = "pass3453",
            Name = "Jack Reacher"
        };
        var user = context.Add(contributor).Entity;
        await context.SaveChangesAsync();
        Assert.NotNull(user);

        AddContributorToProjectCommand updateProjectcommand = new AddContributorToProjectCommand(prorandom.Id, user.Id);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, $"api/projects/contributors")
        {
            Content = new StringContent(JsonSerializer.Serialize(updateProjectcommand), Encoding.UTF8,
                "application/json"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<Project>(jsonString, options);
        Assert.NotNull(result);
    }

    /// <summary>
    /// Adds a task to a project via API and validates persistence.
    /// </summary>
    [Fact, TestPriority(12)]
    public async Task AddTaskToProjectTest()
    {
        var prorandom = await context.Projects.FirstOrDefaultAsync();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        ProjectTask newTask = new()
        {
            Title = "PINN",
            Description = "PhysicsInformedNeuralNetwors",
        };
        var addTaskToProject = new AddProjectTaskCommand(prorandom.Id, newTask);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, $"api/projects/tasks")
        {
            Content = new StringContent(JsonSerializer.Serialize(addTaskToProject), Encoding.UTF8, "application/json"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<Project>(jsonString, options);
        Assert.NotNull(result);
        Assert.NotNull(result.Tasks);
        Assert.NotEmpty(result.Tasks);
        var tasks = result.Tasks.ToList();
        var task = tasks.FirstOrDefault(t => t.Title == newTask.Title);
        Assert.NotNull(task);
    }
[Fact, TestPriority(13)]
    public async Task AddCommentToProjectTest()
    {
        var prorandom = await context.Projects.FirstOrDefaultAsync();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        Comment comment = new Comment()
        {
            Content = "a test comment",
        };
        var command = new AddProjectCommentCommand(prorandom.Id, comment);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, $"api/projects/{prorandom.Id}/comments")
        {
            Content = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<Project>(jsonString, options);
        Assert.NotNull(result);
        Assert.NotNull(result.Comments);
        Assert.NotEmpty(result.Comments);
        var tasks = result.Comments.ToList();
        var firscomment= tasks.FirstOrDefault(t => t.Content == comment.Content);
        Assert.NotNull(firscomment);
        Assert.Equal(comment.Content, firscomment.Content);
    }
    #endregion

    #region DeleteRequests

    /// <summary>
    /// Deletes a project task by ID and ensures removal.
    /// </summary>
    [Fact, TestPriority(14)]
    public async Task DeleteProjectTaskByIdTest()
    {
        var prorandom = context.Projects.Include(u => u.Tasks).FirstOrDefault();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        Assert.NotNull(prorandom.Tasks);
        var task = prorandom.Tasks.FirstOrDefault(t => t.projectId == prorandom.Id);
        await context.SaveChangesAsync();
        Assert.NotNull(task);
        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"api/projects/{prorandom.Id}/tasks/{task.Id}")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(deleteRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var taskFound = context.Tasks.FirstOrDefault(t => t.Id == task.Id);
        await context.SaveChangesAsync();
        Assert.Null(taskFound);
    }

    /// <summary>
    /// Deletes a contributor from a project by ID and ensures removal.
    /// </summary>
    [Fact, TestPriority(15)]
    public async Task DeleteContributorTaskByIdTest()
    {
        var prorandom = await context.Projects.Include(u => u.Contributors).FirstOrDefaultAsync();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        Assert.NotNull(prorandom.Contributors);
        var user = prorandom.Contributors.FirstOrDefault(t => t.ProjectId == prorandom.Id);
        Assert.NotNull(user);
        await context.SaveChangesAsync();
        var deleteRequest =
            new HttpRequestMessage(HttpMethod.Delete, $"api/projects/{prorandom.Id}/contributors/{user.Id}")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
            };
        var response = await Client.SendAsync(deleteRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var result = context.Users.FirstOrDefault(p => p.ProjectId == prorandom.Id && p.Id == user.Id);
        await context.SaveChangesAsync();
        Assert.Null(result);
    }

    #endregion

    #region UpdateRequests

    [Fact, TestPriority(16)]
    public async Task SetProjectDeadlineTest()
    {
        var prorandom = await context.Projects.FirstOrDefaultAsync();
        await context.SaveChangesAsync();
        Assert.NotNull(prorandom);
        var updateDeadlinecommand = new SetProjectDeadlineCommand(prorandom.Id,DateTime.Parse("2027-10-10"));
        var postRequest = new HttpRequestMessage(HttpMethod.Put, $"api/projects/{prorandom.Id}")
        {
            Content = new StringContent(JsonSerializer.Serialize(updateDeadlinecommand), Encoding.UTF8,
                "application/json"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(postRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var jsonString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<Project>(jsonString, options);
        Assert.NotNull(result);
        Assert.NotNull(result.Deadline);
        Assert.Equal(DateTime.Parse("2027-10-10"), result.Deadline);
    }
    #endregion
    #endregion

    /// <summary>
    /// Tests the system's resilience against high-frequency requests.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="TeamcollborationHub.server.Middlewares.IpBasedRateLimiter"/> middleware 
    /// correctly returns a <c>429 Too Many Requests</c> status when thresholds are exceeded.
    /// </remarks>
    [Fact, TestPriority(17)]
    public async Task RateLimitTest()
    {
        LoginRequestDto request = new("lahcen30@gmail.com", "password123");
        string json = JsonSerializer.Serialize(request);
        var postRequest = new HttpRequestMessage(HttpMethod.Post, "/login")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
            Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) }
        };
        var response = await Client.SendAsync(postRequest);
        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }
}