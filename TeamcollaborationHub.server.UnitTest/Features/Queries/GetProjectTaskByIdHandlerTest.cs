using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectTaskById;

namespace TeamcollaborationHub.server.UnitTest.Features.Queries;

[TestFixture]
public class GetProjectTaskByIdHandlerTest
{
    [Test]
    public void GetProjectTaskById_ShouldReturnProjectTask()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TdbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new TdbContext(options);
        context.Tasks.Add(new ProjectTask { Id = 1, Title = "Task 1" });
        context.SaveChanges();
        var handler = new GetProjectTaskByIdQueryHandler(context);
        // Act
        var result = handler.Handle(new GetProjectTaskByIdQuery(1), CancellationToken.None).Result;
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual("Task 1", result.Title);
    }
}

