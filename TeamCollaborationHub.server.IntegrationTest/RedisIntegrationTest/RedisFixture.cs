using StackExchange.Redis;
using Testcontainers.Redis;


namespace TeamCollaborationHub.server.IntegrationTest.RedisIntegrationTest;

public class RedisFixture : IAsyncLifetime
{
    public RedisContainer RedisContainer { get; } = new RedisBuilder()
        .WithImage("redis:6.2-alpine")
        .Build();

    public IConnectionMultiplexer? Connection { get; private set; }

    public async Task InitializeAsync()
    {
        await RedisContainer.StartAsync();
        Connection = await ConnectionMultiplexer.ConnectAsync(RedisContainer.GetConnectionString());
    }
    public async Task DisposeAsync()
    {
        await Connection?.CloseAsync()!;
        await RedisContainer.DisposeAsync();
    }
}
