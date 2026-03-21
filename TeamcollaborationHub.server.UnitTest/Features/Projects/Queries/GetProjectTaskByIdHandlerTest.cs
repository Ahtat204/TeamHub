using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectTaskById;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Queries;

[TestFixture, TestOf(typeof(GetProjectTaskByIdQueryHandler))]
public class GetProjectTaskByIdHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }
    [Test]
    public void GetProjectTaskById_ShouldReturnProjectTask()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        context.Tasks.Add(new ProjectTask { Id = 1, Title = "Task 1" });
        context.SaveChanges();
        var handler = new GetProjectTaskByIdQueryHandler(context);
        var result = handler.Handle(new GetProjectTaskByIdQuery(1), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(1, Is.EqualTo(result.Id));
        Assert.That(result.Title, Is.EqualTo("Task 1"));
        context.Database.EnsureDeleted();
    }

    [Test]
    public void GetProjectTaskById_ShouldThrowNotFoundException_WhenProjectTaskNotFound()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        context.Tasks.Add(new ProjectTask { Id = 3, Title = "Task 1" });
        context.SaveChanges();
        var handler = new GetProjectTaskByIdQueryHandler(context);
        //var result = handler.Handle(new GetProjectTaskByIdQuery(2), CancellationToken.None).Result;
        Assert.That(() => handler.Handle(new(2), CancellationToken.None), Throws.Exception.TypeOf<NotFoundException<ProjectTask>>());
        context.Database.EnsureDeleted();
    }
}