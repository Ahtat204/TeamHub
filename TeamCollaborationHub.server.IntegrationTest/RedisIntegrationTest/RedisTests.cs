using StackExchange.Redis;
using Xunit.Abstractions;
using Xunit.Sdk;
using TeamcollborationHub.server.Entities;
using System.Text.Json;
using TeamcollborationHub.server.Enums;

namespace TeamCollaborationHub.server.IntegrationTest.RedisIntegrationTest;

[Collection("Redis Collection")]
public class RedisTests : IAsyncLifetime, IClassFixture<RedisFixture>
{
    private readonly Project _project = new Project
    {
        Id = 1,
        Name = "Test Project",
        Description = "This is a test project",
        status = ProjectStatus.Completed
    };

    private readonly ITestOutputHelper _output = new TestOutputHelper();
    private readonly RedisFixture _redisFixture;
    private readonly IDatabase _database;

    public RedisTests(RedisFixture redisFixture)
    {
        _redisFixture = redisFixture;
        _database = _redisFixture.Connection.GetDatabase();
    }

    public async Task InitializeAsync()
    {
        await _database.ExecuteAsync("FLUSHALL");
        
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Test_SetAndGetValue()
    {
        string key = _project.Id.ToString();
        string value = JsonSerializer.Serialize(_project);
        await _database.StringSetAsync(key, value);
        var retrievedValue = await _database.StringGetAsync(key);
        /*_output.WriteLine("Redis initialized" + "Connected to Redis" +
                          _redisFixture.RedisContainer.GetConnectionString());*/
        Assert.NotNull(_redisFixture.Connection);
        Assert.Equal(value, retrievedValue);
    }

    [Fact]
    public async Task Test_KeyDoesNotExistAfterFlush()
    {
        var exists = await _database.KeyExistsAsync(_project.Id.ToString());
        Assert.False(exists);
    }
}