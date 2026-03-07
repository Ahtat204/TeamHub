using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;

namespace TeamcollaborationHub.server.UnitTest.Features.Queries;

[TestFixture]
public class GetProjectByIdHandlerTest
{
    [Test]
    public void GetProjectById_ShouldReturnProject()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TdbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new TdbContext(options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        context.SaveChanges();
        var handler = new GetProjectByIdQueryHandler(context);
        // Act
        var result = handler.Handle(new GetProjectByIdQuery(1), CancellationToken.None).Result;
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(project.Id, result.Id);
        Assert.AreEqual(project.Name, result.Name);
    }
}
