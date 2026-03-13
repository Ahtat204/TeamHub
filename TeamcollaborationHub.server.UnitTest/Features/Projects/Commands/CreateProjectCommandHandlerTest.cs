using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Features.Projects.Commands.CreateProject;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture]
[TestOf(typeof(CreateProjectCommandHandler))]
public class CreateProjectCommandHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Test]
    public void CreatesProject_ShouldReturnTheCreatedProject()
    {
        Assert.That(_options, Is.Not.Null);
        var project = new Project
        {
            Deadline = DateTime.Today,
            Id = 1,
            Name = "Project 1",
            Description = "do I know you",
            status = ProjectStatus.InProgress
        };
        var context = new TdbContext(_options);
        var handler = new CreateProjectCommandHandler(context);
        var result = handler
            .Handle(new CreateProjectCommand("Project 1", "do I know you", [], ProjectStatus.Started, DateTime.Today),
                CancellationToken.None).Result;
        Assert.NotNull(result);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Name, Is.EqualTo("Project 1"));
        Assert.That(result.Description, Is.EqualTo("do I know you"));
        context.Database.EnsureDeleted();
    }
    
}