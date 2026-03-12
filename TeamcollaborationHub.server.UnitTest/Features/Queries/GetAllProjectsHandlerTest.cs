
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Queries;

[TestFixture]
public class GetAllProjectsHandlerTest
{

    [Test]
    public void GetAllProjects_ShouldReturnListOfProjects()
    {
        var options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase").Options;
        using var context = new TdbContext(options);
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
    }
}
