using DotNet.Testcontainers;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit.Abstractions;


namespace TeamCollaborationHub.server.IntegrationTest.RedisIntegrationTest;

public class RedisFixture : IAsyncLifetime
{
    public RedisContainer RedisContainer;

  
    public IConnectionMultiplexer Connection;

    public RedisFixture()
    {
    }

    public async Task InitializeAsync()
    {
        
        RedisContainer = new RedisBuilder()
            .WithImage("redis:6.2-alpine")
            .Build();
        await RedisContainer.StartAsync();
        Connection = await ConnectionMultiplexer.ConnectAsync(RedisContainer.GetConnectionString());
       
    }

    public async Task DisposeAsync()
    {
        await Connection.CloseAsync()!;
        await RedisContainer.DisposeAsync();
    }
}