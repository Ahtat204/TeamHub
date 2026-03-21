using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture, TestOf(typeof(RemoveProjectTaskHandler))]
public class RemoveProjectTaskHandlerTest
{

    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }


    [Test]
    public void RemoveProjectTask()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        Project pro = new()
        {
            Deadline = DateTime.Today,
            Id = 1,
            Name = "Project 1",
            Description = "do I know you",
            Status = ProjectStatus.InProgress
        };
        ProjectTask projectTask = new()
        {
            Id = 1,
            project = pro,
            Description = "do I know you",
            projectId = pro.Id,

        };
        context.Projects.Add(pro);
        context.Tasks.Add(projectTask);
        context.SaveChanges();
        var handler = new RemoveProjectTaskHandler(context);
        var result = handler.Handle(new RemoveProjectTaskCommand(projectTask.Id), CancellationToken.None);
        var task = context.Tasks.SingleOrDefault(t => t.Id == projectTask.Id);
        Assert.That(task, Is.Null);
        context.Database.EnsureDeleted();

    }
}