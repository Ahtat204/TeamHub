using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Queries;

[TestFixture]

public class GetProjectContributorByIdHandlerTest
{
    [Test]
    public void GetProjectContributorById_ShouldReturnContributor()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TdbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new TdbContext(options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        var contributor = new User { Id = 1, Name = "Contributor 1", ProjectId = project.Id };
        context.Users.Add(contributor);
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
        Assert.AreEqual(1, projectResult.contributor.Count());
        var contributorResult = projectResult.contributor.First();
        Assert.AreEqual(contributor.Id, contributorResult.Id);
        Assert.AreEqual(contributor.Name, contributorResult.Name);
    }
}
