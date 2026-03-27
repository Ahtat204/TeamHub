using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectContributors;


namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Queries;

[TestFixture]

public class GetAllProjectContributorsHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }
    [Test]
    public void GetAllProjectContributors_ShouldReturnListOfContributors()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        context.Users.AddRange(
            new User { Id = 1, Name = "Contributor 1", ProjectId = project.Id },
            new User { Id = 2, Name = "Contributor 2", ProjectId = project.Id }
        );
        context.SaveChanges();
        var handler = new GetAllProjectContributorsQueryHandler(context);
        var result = handler.Handle(new GetAllProjectContributorsQuery(1), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(2));
        var projectResult = result.First();
        Assert.IsNotNull(projectResult);
        Assert.That(projectResult.Name, Contains.Substring("Contributor"));
        context.Database.EnsureDeleted();
    }

    [Test]
    public void GetAllProjectContributors_ShouldReturnEmptyListOfContributors()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var handler = new GetAllProjectContributorsQueryHandler(context);
        var result = handler.Handle(new GetAllProjectContributorsQuery(1), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(0));
        context.Database.EnsureDeleted();
    }
}
