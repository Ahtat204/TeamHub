using TeamcollborationHub.server.Features.Projects.Commands.SetProjectDeadline;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture]
[TestOf(typeof(SetProjectDeadlineCommandHandler))]
public class SetProjectDeadlineCommandHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Test]
    public async Task SetProjectDeadLineTest()
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
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        var handler = new SetProjectDeadlineCommandHandler(context);
        var result = await handler.Handle(new SetProjectDeadlineCommand(project.Id, DateTime.Today),
            CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(project.Id));
            Assert.That(result.Deadline, Is.EqualTo(DateTime.Today));
        });
        await context.Database.EnsureDeletedAsync();
    }

    [Test]
    public async Task SetProjectDeadlineTest_shouldThrowInvalidDateException()
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
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        var handler = new SetProjectDeadlineCommandHandler(context);
        Assert.That(() => handler.Handle(new SetProjectDeadlineCommand(project.Id, DateTime.Parse("2026-03-26 14:30")),
            CancellationToken.None), Throws.Exception.TypeOf<InvalidDateException>());
        await context.Database.EnsureDeletedAsync();
    }
}