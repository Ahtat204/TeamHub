
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Queries;

[TestFixture]

public class GetAllProjectTasksHandlerTest
{
    [Test]
    public void GetAllProjectTasks_ShouldReturnListOfProjectTasks()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TdbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        using var context = new TdbContext(options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        context.Tasks.AddRange(
            new ProjectTask { Id = 1, Title = "Task 1", projectId = project.Id },
            new ProjectTask { Id = 2, Title = "Task 2", projectId = project.Id }
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
        Assert.AreEqual(2, projectResult.Tasks.Count());
        Assert.IsTrue(projectResult.Tasks.Any(t => t.Title == "Task 1"));
        Assert.IsTrue(projectResult.Tasks.Any(t => t.Title== "Task 2"));
    }
}
