using Microsoft.Extensions.DependencyInjection;
using TeamcollborationHub.server.Configuration;


namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;

public abstract class BaseIntegrationTestFixture : IClassFixture<TeamHubApplicationFactory<Program, TdbContext>>
{
   private readonly TeamHubApplicationFactory<Program, TdbContext> AppFactory;
   protected readonly IServiceScope scope;
   

   protected BaseIntegrationTestFixture(TeamHubApplicationFactory<Program, TdbContext> appFactory)
   {
      AppFactory =appFactory ?? throw new ArgumentNullException(nameof(appFactory));
      
      scope = AppFactory.Services.CreateScope();
      var context=scope.ServiceProvider.GetRequiredService<TdbContext>();
      context.Database.EnsureCreated();
      
   }
}
