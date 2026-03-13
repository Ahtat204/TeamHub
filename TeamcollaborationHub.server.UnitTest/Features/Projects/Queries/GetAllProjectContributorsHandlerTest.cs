using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Queries;

[TestFixture]

public class GetAllProjectContributorsHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options=new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
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
        var handler = new GetAllProjectsQueryHandler(context);
        var result = handler.Handle(new GetAllProjectsQuery(), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(1));
        var projectResult = result.First();
        Assert.That(projectResult.Id, Is.EqualTo(project.Id));
        Assert.That(projectResult.Name, Is.EqualTo(project.Name));
        Assert.IsNotNull(projectResult);
        Assert.IsNotNull(projectResult.contributor);
        Assert.That(projectResult.contributor, Has.Count.EqualTo(2));
        Assert.IsTrue(projectResult.contributor.Any(c => c.Name == "Contributor 1"));
        Assert.IsTrue(projectResult.contributor.Any(c => c.Name == "Contributor 2"));
        context.Database.EnsureDeleted();
    }

    [Test]
    public void GetAllProjectContributors_ShouldReturnEmptyListOfContributors()
    {
        using var context = new TdbContext(_options);
        var handler = new GetAllProjectsQueryHandler(context);
        var result = handler.Handle(new GetAllProjectsQuery(), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(0));
        context.Database.EnsureDeleted();
    }
}
