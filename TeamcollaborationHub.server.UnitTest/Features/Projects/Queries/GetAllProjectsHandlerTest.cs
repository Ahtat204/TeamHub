using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Queries;

[TestFixture]
public class GetAllProjectsHandlerTest
{

    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options=new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }
    [Test]
    public void GetAllProjects_ShouldReturnListOfProjects()
    {
       
        using var context = new TdbContext(_options);
        context.Projects.AddRange(
            new Project { Id = 1, Name = "Project 1" },
            new Project { Id = 2, Name = "Project 2" }
        );
        context.SaveChanges();
        var handler = new GetAllProjectsQueryHandler(context);
        var result = handler.Handle(new(), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.IsTrue(result.Any(p => p.Name == "Project 1"));
        Assert.IsTrue(result.Any(p => p.Name == "Project 2"));
        context.Database.EnsureDeleted();
    }

    [Test]
    public void GetAllProjects_ShouldReturnEmptyListOfProjects()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var handler = new GetAllProjectsQueryHandler(context);
        var result = handler.Handle(new(), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(0));
        context.Database.EnsureDeleted();
        
    }
}
