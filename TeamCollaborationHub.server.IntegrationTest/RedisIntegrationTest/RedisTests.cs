using System.Threading.Tasks;
using StackExchange.Redis;

namespace TeamCollaborationHub.server.IntegrationTest.RedisIntegrationTest;

[CollectionDefinition("Redis Collection")]
public class RedisTests:IAsyncLifetime
{
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
        const string key = "test-key";
        const string value = "test-value";
        await _database.StringSetAsync(key, value);
        var retrievedValue = await _database.StringGetAsync(key);
        Assert.Equal(value, retrievedValue);
    }
    [Fact]
    public async Task Test_KeyDoesNotExistAfterFlush()
    {
        var exists = await _database.KeyExistsAsync("test-key");
        Assert.False(exists);
    }
 
}