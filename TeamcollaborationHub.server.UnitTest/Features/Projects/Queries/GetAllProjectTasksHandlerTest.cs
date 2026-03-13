using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Queries;

[TestFixture]
public class GetAllProjectTasksHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options=new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }
    [Test]
    public void GetAllProjectTasks_ShouldReturnListOfProjectTasks()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        context.Tasks.AddRange(
            new ProjectTask { Id = 1, Title = "Task 1", projectId = project.Id },
            new ProjectTask { Id = 2, Title = "Task 2", projectId = project.Id }
        );
        context.SaveChanges();
        var handler = new GetAllProjectsQueryHandler(context);
        var result = handler.Handle(new GetAllProjectsQuery(), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(1));
        var projectResult = result.First();
        Assert.That(projectResult.Id, Is.EqualTo(project.Id));
        Assert.That(projectResult.Name, Is.EqualTo(project.Name));
        Assert.That(projectResult.Tasks,Is.Not.Null);
        Assert.That(projectResult.Tasks, Has.Count.EqualTo(2));
        Assert.IsTrue(projectResult.Tasks.Any(t => t.Title == "Task 1"));
        Assert.That(projectResult.Tasks.Any(t => t.Title== "Task 2"), Is.True);
        context.Database.EnsureDeleted();
    }

    [Test]
    public void GetProjectContributorById_ShouldReturnEmptyListOfTasks()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var handler = new GetAllProjectsQueryHandler(context);
        var result = handler.Handle(new GetAllProjectsQuery(), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(0));
        context.Database.EnsureDeleted();
        
    }
}
