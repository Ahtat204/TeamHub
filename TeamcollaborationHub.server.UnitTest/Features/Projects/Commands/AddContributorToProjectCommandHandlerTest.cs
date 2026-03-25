using TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture]
[TestOf(typeof(AddContributorToProjectCommandHandler))]
public class AddContributorToProjectCommandHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }



    [Test]
    public void AddContributorToProjectTest()
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
        User contributor = new()
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
        var handler = new AddContributorToProjectCommandHandler(context);
        var result = handler.Handle(new AddContributorToProjectCommand(project.Id, contributor.Id),
            CancellationToken.None);
        var check = context.Projects.Include(pro => pro.Contributors).SingleOrDefault(p => p.Id == project.Id);
        Assert.That(check, Is.Not.Null);
        Assert.That(check.Contributors, Is.Not.Null);
        Assert.That(check.Contributors, Has.Count.EqualTo(1));
        Assert.That(check.Contributors.Single().Email, Is.EqualTo(contributor.Email));
        context.Database.EnsureDeleted();
    }

    [Test]
    public void AddContributorToProjectTest_ShouldThrowNotFoundExceptiontypeofUser()
    {
        Assert.That(_options, Is.Not.Null);
        var context = new TdbContext(_options);
        var handler = new AddContributorToProjectCommandHandler(context);
        Assert.That(() => handler.Handle(new(1,1), CancellationToken.None), Throws.Exception.TypeOf<NotFoundException<User>>());
        context.Database.EnsureDeleted();
    }

    [Test]
    public void AddContributorToProjectTest_ShouldThrowNotFoundExceptionTypeofProject()
    {
        Assert.That(_options, Is.Not.Null);
        var context = new TdbContext(_options);
        User contributor = new()
        {
            Id = 4,
            Email = "lahcen28ahtat@gmail",
            Password = "pass3453",
            Name = "Jack Reacher"
        };
        context.Users.Add(contributor);
        var handler = new AddContributorToProjectCommandHandler(context);
        var result = handler.Handle(new(4,contributor.Id), CancellationToken.None);
        Assert.That(() => handler.Handle(new(4,contributor.Id), CancellationToken.None), Throws.Exception.TypeOf<NotFoundException<Project>>());
        context.Database.EnsureDeleted();
    }
}