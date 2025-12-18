using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;


namespace TeamCollaborationHub.server.IntegrationTest.RedisIntegrationTest;

public class RedisFixture : IAsyncLifetime
{
    public RedisContainer RedisContainer { get; }
    public IConnectionMultiplexer Connection { get; private set; }
    public RedisFixture()
    {
       
        RedisContainer = new RedisBuilder()
            .WithImage("redis:6.2-alpine")
            .Build();
    }
    public async Task InitializeAsync()
    {
        await RedisContainer.StartAsync();
        Connection = await ConnectionMultiplexer.ConnectAsync(RedisContainer.GetConnectionString());
    }
    public async Task DisposeAsync()
    {
        await Connection.CloseAsync();
        await RedisContainer.DisposeAsync();
    }
}
