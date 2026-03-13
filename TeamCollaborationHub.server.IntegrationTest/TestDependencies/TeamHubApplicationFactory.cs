using System.Net;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Testcontainers.Redis;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Middlewares;
using TeamcollborationHub.server.Repositories.UserRepository;
using Testcontainers.MsSql;

namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;

    public class TeamHubApplicationFactory<T,TP> :WebApplicationFactory<T> ,IAsyncLifetime where T : Program where TP : DbContext
    {
       
        
        private static readonly MsSqlContainer SqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Password1").WithName("sql_server_container").WithEnvironment("ACCEPT_EULA","sa")
            .WithPortBinding(1433).WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
            .Build();

        private static readonly RedisContainer _redisContainer= new RedisBuilder()
            .WithImage("redis:latest")
            .Build();
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddSingleton<IDatabase>(sp =>
                {
                    var multiplexer = ConnectionMultiplexer.ConnectAsync(_redisContainer.GetConnectionString()).Result;

                   return multiplexer.GetDatabase(); // cheap pass-through
                });
                services.AddSingleton(
                    LuaScript.Prepare("local requests = redis.call('INCR', KEYS[1])\n\nif requests == 1 then\n    redis.call('EXPIRE', KEYS[1], ARGV[2])\nend\n\nif requests > tonumber(ARGV[1]) then\n    return 1\nelse\n    return 0\nend")
                );
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TdbContext>));
                if (descriptor is not null) { services.Remove(descriptor); }
                services.AddDbContext<TdbContext>(options => { options.UseSqlServer(SqlServerContainer.GetConnectionString()); });
            });
            builder.UseEnvironment("Testing");
        }
        public async Task InitializeAsync()
        {
          
            await SqlServerContainer.StartAsync();
            await _redisContainer.StartAsync();
        }

        public new async Task DisposeAsync() {
            await SqlServerContainer.StopAsync();
            await _redisContainer.StopAsync();
        }
       
    }
