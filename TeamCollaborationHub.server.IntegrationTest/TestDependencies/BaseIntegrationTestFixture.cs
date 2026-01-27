using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamcollborationHub.server.Configuration;


namespace TeamCollaborationHub.server.IntegrationTest.TestDependencies;

public abstract class BaseIntegrationTestFixture : IClassFixture<TeamHubApplicationFactory<Program, TDBContext>>
{
   private readonly TeamHubApplicationFactory<Program, TDBContext> AppFactory;
   protected readonly IServiceScope scope;
   protected readonly HttpClient HttpClient;
   private readonly ILogger<BaseIntegrationTestFixture> _logger;
   

   protected BaseIntegrationTestFixture(TeamHubApplicationFactory<Program, TDBContext> appFactory)
   {
      AppFactory =appFactory ?? throw new System.ArgumentNullException(nameof(appFactory));
      scope = AppFactory.Services.CreateScope();
      HttpClient = appFactory.CreateClient();
      var context=scope.ServiceProvider.GetRequiredService<TDBContext>();
      var part = scope.ServiceProvider.GetRequiredService<ApplicationPartManager>();
      var feature = new ControllerFeature();
      part.PopulateFeature(feature);
      _logger = scope.ServiceProvider.GetRequiredService<ILogger<BaseIntegrationTestFixture>>();
      foreach (var controller in feature.Controllers)
      {
         _logger.LogInformation($"Registering controller {controller.Name}");
      }
      context.Database.EnsureCreated();
      
   }
}
