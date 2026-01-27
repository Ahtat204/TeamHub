using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Repositories.UserRepository;
using Testcontainers.MsSql;

namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;

    public class TeamHubApplicationFactory<T,TP> :WebApplicationFactory<T> ,IAsyncLifetime where T : Program where TP : DbContext
    {
       
        private static readonly MsSqlContainer _sqlServerContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Password1").WithName("sql_server_container").WithEnvironment("ACCEPT_EULA","sa")
            .WithPortBinding(1433).WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(1433))
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddDbContext<TDBContext>(options => { options.UseSqlServer(_sqlServerContainer.GetConnectionString()); });
            });
        }
        public async Task InitializeAsync()
        {
          
            await _sqlServerContainer.StartAsync();
        }

        public new async Task DisposeAsync() => await _sqlServerContainer.StopAsync();
       
    }
