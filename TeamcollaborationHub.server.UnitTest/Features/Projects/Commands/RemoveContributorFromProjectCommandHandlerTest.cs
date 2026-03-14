using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Features.Projects.Commands.RemoveContributorFromProject;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture,TestOf(typeof(RemoveContributorFromProjectCommandHandler))]
public class RemoveContributorFromProjectCommandHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }


    [Test]
    public void RemoveContributorFromProject()
    {
        
        Assert.That(_options,Is.Not.Null);
        var context = new TdbContext(_options);
        Project project = new()
        {
            Deadline = DateTime.Today,
            Id = 1,
            Name = "Project 1",
            Description = "do I know you",
            Status = ProjectStatus.InProgress
        };
        User contributor = new ()
        {
            Id = 4,
            Email = "lahcen28ahtat@gmail",
            Password = "pass3453",
            project = project,
            ProjectId = project.Id,
            Name = "Jack Reacher"
        };
        context.Projects.Add(project);
        context.Users.Add(contributor);
        var handler = new RemoveContributorFromProjectCommandHandler(context);
        var result = handler.Handle(new RemoveContributorFromProjectCommand(project.Id, contributor.Id),
            CancellationToken.None);
        var check = context.Users.FirstOrDefault(p => p.ProjectId == project.Id && p.Id==contributor.Id);
        Assert.That(check, Is.Null);
        context.Database.EnsureDeleted();
    }
}