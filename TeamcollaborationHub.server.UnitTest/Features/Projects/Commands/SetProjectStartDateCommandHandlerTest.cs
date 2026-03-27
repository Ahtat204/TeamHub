using System.Runtime.InteropServices.JavaScript;
using TeamcollborationHub.server.Features.Projects.Commands.SetProjectStartDate;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture]
[TestOf(typeof(SetProjectStartDateCommandHandler))]
public class SetProjectStartDateCommandHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Test]
    public async Task SetProjectStartDateTest()
    {
        Assert.That(_options, Is.Not.Null);
        var context = new TdbContext(_options);
        Assert.That(context, Is.Not.Null);
        Project project = new()
        {
            Deadline = DateTime.Parse("2027-10-16"),
            Id = 1,
            Name = "Project 1",
            Description = "do I know you",
            Status = ProjectStatus.NotStarted
        };
        var startDate = DateTime.Today;
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        var handler = new SetProjectStartDateCommandHandler(context);
        var result = await handler.Handle(new SetProjectStartDateCommand(project.Id,startDate), CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CreatedDateTime, Is.EqualTo(startDate));
        await context.Database.EnsureDeletedAsync();
    }
    [TestCase("2025-10-16")]
    [TestCase("2026-03-26")]
    [Test]
    public async Task SetProjectStartDate_ShouldThrowInvalidDateException(string testDate)
    {
        Assert.That(_options, Is.Not.Null);
        var context = new TdbContext(_options);
        Assert.That(context, Is.Not.Null);
        Project project = new()
        {
            Deadline = DateTime.Parse("2027-10-16"),
            Id = 1,
            Name = "Project 1",
            Description = "do I know you",
            Status = ProjectStatus.InProgress
        };
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        var startDate = DateTime.Parse(testDate);
        var handler = new SetProjectStartDateCommandHandler(context);
        Assert.That(()=>handler.Handle(new SetProjectStartDateCommand(project.Id,startDate), CancellationToken.None),Throws.Exception.TypeOf<InvalidDateException>());
        await context.Database.EnsureDeletedAsync();
    }

    [TestCase("2027-11-26")]
    [TestCase("2027-10-17")]
    [Test]
    public async Task SetProjectStartDate2_ShouldThrowInvalidDateException(string testDate)
    {
        Assert.That(_options, Is.Not.Null);
        var context = new TdbContext(_options);
        Assert.That(context, Is.Not.Null);
        Project project = new()
        {
            Deadline = DateTime.Parse("2027-10-16"),
            Id = 1,
            Name = "Project 1",
            Description = "do I know you",
            Status = ProjectStatus.InProgress
        };
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        var startDate = DateTime.Parse(testDate);
        var handler = new SetProjectStartDateCommandHandler(context);
        Assert.That(()=>handler.Handle(new SetProjectStartDateCommand(project.Id,startDate), CancellationToken.None),Throws.Exception.TypeOf<InvalidDateException>());
        await context.Database.EnsureDeletedAsync();
    }
}