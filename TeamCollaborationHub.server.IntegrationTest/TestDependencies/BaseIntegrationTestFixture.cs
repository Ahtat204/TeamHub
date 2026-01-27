using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TeamcollborationHub.server.Configuration;


namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;

public class BaseIntegrationTestFixture : IClassFixture<TeamHubApplicationFactory<Program, TDBContext>>
{
   public readonly TeamHubApplicationFactory<Program, TDBContext> AppFactory;
   public readonly TDBContext TdbContext;

   public BaseIntegrationTestFixture(TeamHubApplicationFactory<Program, TDBContext> appFactory)
   {
      AppFactory =appFactory ?? throw new System.ArgumentNullException(nameof(appFactory));
      var scope = AppFactory.Services.CreateScope();
      TdbContext = scope.ServiceProvider.GetRequiredService<TDBContext>();
   }
}
