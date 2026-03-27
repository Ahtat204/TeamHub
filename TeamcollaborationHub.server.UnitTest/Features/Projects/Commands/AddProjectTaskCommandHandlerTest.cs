using TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;
using TeamcollborationHub.server.Enums;
namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture]
[TestOf(typeof(AddProjectTaskCommandHandler))]
public class AddProjectTaskCommandHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }


    [Test]
    public async Task AddTaskToProjectTest()
    {
        Assert.That(_options, Is.Not.Null);
        var context = new TdbContext(_options);
        Project project = new()
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
            Description = "do I know you",
        };
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        var handler = new AddProjectTaskCommandHandler(context);
        var result = await handler.Handle(new AddProjectTaskCommand(project.Id, projectTask), CancellationToken.None);

        Assert.That(result?.Tasks, Is.Not.Null);
        Assert.That(result.Tasks.Count, Is.EqualTo(1));
        await context.Database.EnsureDeletedAsync();
    }

    [Test]
    public async Task AddTaskToProjectTest_ShouldThrowArgumentNullException()
    {
        Assert.That(_options, Is.Not.Null);
        var context = new TdbContext(_options);
        ProjectTask? nullProjectTask = null;
        var handler = new AddProjectTaskCommandHandler(context);
        Assert.That(()=>handler.Handle(new AddProjectTaskCommand(1, nullProjectTask), CancellationToken.None), Throws.InstanceOf<ArgumentNullException>());
        await context.Database.EnsureDeletedAsync();
        
    }
}