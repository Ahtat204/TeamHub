using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;


namespace TeamCollaborationHub.server.IntegrationTest.RedisIntegrationTest;

public class RedisFixture : IAsyncLifetime
{// RedisContainer is a dedicated class provided by Testcontainers for Redis.
    public RedisContainer RedisContainer { get; }
    
    // This connection will be used by all tests in our collection.
    public IConnectionMultiplexer Connection { get; private set; }
    public RedisFixture()
    {
        // Configure the Redis container; pin the image version to "redis:7.0" for stability.
        RedisContainer = new RedisBuilder()
            .WithImage("redis:7.0")
            .Build();
    }
    // Called once before any tests run.
    public async Task InitializeAsync()
    {
        // Start the container.
        await RedisContainer.StartAsync();
        // Connect to Redis using the container's connection string.
        Connection = await ConnectionMultiplexer.ConnectAsync(RedisContainer.GetConnectionString());
    }
    // Called once after all tests have finished.
    public async Task DisposeAsync()
    {
        // Cleanly close the connection.
        await Connection.CloseAsync();
        // Stop and dispose of the container.
        await RedisContainer.DisposeAsync();
    }
}
