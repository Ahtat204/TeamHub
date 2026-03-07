using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Queries;

[TestFixture]

public class GetAllProjectContributorsHandlerTest
{
    [Test]
    public void GetAllProjectContributors_ShouldReturnListOfContributors()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TdbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new TdbContext(options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        context.Users.AddRange(
            new User { Id = 1, Name = "Contributor 1", ProjectId = project.Id },
            new User { Id = 2, Name = "Contributor 2", ProjectId = project.Id }
        );
        context.SaveChanges();
        var handler = new GetAllProjectsQueryHandler(context);
        // Act
        var result = handler.Handle(new GetAllProjectsQuery(), CancellationToken.None).Result;
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        var projectResult = result.First();
        Assert.AreEqual(project.Id, projectResult.Id);
        Assert.AreEqual(project.Name, projectResult.Name);
        Assert.AreEqual(2, projectResult.contributor.Count());
        Assert.IsTrue(projectResult.contributor.Any(c => c.Name == "Contributor 1"));
        Assert.IsTrue(projectResult.contributor.Any(c => c.Name == "Contributor 2"));
    }
}
