using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using  TeamCollaborationHub.server.IntegrationTest.TestDependencies;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Repositories.UserRepository;

namespace TeamCollaborationHub.server.IntegrationTest;

public class AuthenticationIntegrationTest:BaseIntegrationTestFixture
{

    private readonly TeamHubApplicationFactory<Program, TDBContext> _applicationFactory;
    private readonly IUserRepository? _userRepository;
    private User user= new ()
    {
        Name = "John Doe",
        Email = "test@test.com",
        Password = "password123",
    };
    public AuthenticationIntegrationTest(TeamHubApplicationFactory<Program, TDBContext> appFactory) : base(appFactory)
    {
        _applicationFactory = appFactory;
        var scope = appFactory.Services.CreateScope();
        _userRepository = scope.ServiceProvider.GetService<IUserRepository>();
    }

    #region DatabaseTests
    [Fact]
    public async Task InsertUserTest()
    {
        
        var result=  await _userRepository?.CreateUser(user);
        var response=await _userRepository?.GetUserByEmail(result?.Email);
        Assert.NotNull(response);
        Assert.Equal(user.Email, response?.Email);
    }


    [Fact]
    public async Task DeleteUserTest()
    {
        var request = await _userRepository?.CreateUser(user);
        var response=await _userRepository?.deleteUser(user.Email);
        var getUserResponse=await _userRepository?.GetUserByEmail(user.Email);
        Assert.NotNull(response);
        Assert.Null(getUserResponse);
    }
    
    #endregion
    
    #region ApiTests

    [Fact]
    public async Task RegisterUserTest()
    {
        
    }
    
    #endregion
   
}