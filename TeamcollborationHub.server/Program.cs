using TeamcollborationHub.server.Services;
using dotenv.net;
using TeamcollborationHub.server.Middlewares;
DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration
    .AddEnvironmentVariables(configureSource: source => { source.Prefix = ".env"; }).AddUserSecrets<Program>().Build();
builder.Services.RegisterServices(configuration);
var app = builder.Build();
app.RegisterMiddlewares();
app.Run();
public partial class Program
{
} // added to solve Can't find <'TeamcollaborationHub\TeamCollaborationHub.server.IntegrationTest\bin\Debug\net8.0\testhost.deps.json'> problem 