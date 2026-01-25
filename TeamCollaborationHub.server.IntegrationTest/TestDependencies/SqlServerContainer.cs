using Microsoft.VisualStudio.TestPlatform.TestHost;
using TeamcollborationHub.server.Configuration;


namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;

public class BaseIntegrationTestFixture(
   TeamHubApplicationFactory<Program, TDBContext> appFactory,
   TDBContext tdbContext)
   : IClassFixture<TeamHubApplicationFactory<Program, TDBContext>>
{
   public readonly TeamHubApplicationFactory<Program,TDBContext> AppFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
   public readonly TDBContext TdbContext = tdbContext ?? throw new ArgumentNullException(nameof(tdbContext));
}
