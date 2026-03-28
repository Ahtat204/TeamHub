using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Features.Projects.Commands.AddProjectComment;
namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Commands;

[TestFixture]
[TestOf(typeof(AddProjectCommentCommandHandler))]
public class AddProjectCommentCommandHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Test]
    public async Task AddCommentToProject()
    {
        Assert.NotNull(_options);
        var context = new TdbContext(_options);
        Project project = new()
        {
            Deadline = DateTime.Today,
            Id = 1,
            Name = "Project 1",
            Description = "do I know you",
            Status = ProjectStatus.InProgress
        };
        await context.Projects.AddAsync(project);
        await context.SaveChangesAsync();
        Comment comment = new Comment()
        {
            Id = 1,
            Content = "a test comment",
        };
        var handler = new AddProjectCommentCommandHandler(context);
        var result = await handler.Handle(new AddProjectCommentCommand(project.Id, comment), CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.Comments);
        Assert.IsNotEmpty(result.Comments);
        var firstComment = result.Comments.First();
        Assert.NotNull(firstComment);
        Assert.That(firstComment.Id, Is.EqualTo(comment.Id));
        await context.Database.EnsureDeletedAsync();
    }


    [Test]
    public async Task AddCommentToProject_ShouldThrowArgumentNullException()
    {
        Assert.NotNull(_options);
        var context = new TdbContext(_options);
        Comment? comment = null;
        var handler = new AddProjectCommentCommandHandler(context);
        Assert.That(async () =>await handler.Handle(new AddProjectCommentCommand(1, comment), CancellationToken.None),
            Throws.ArgumentNullException);
    }

    [Test]
    public async Task AddCommentToProject_ShouldThrowNotFoundException()
    {
        Assert.NotNull(_options);
        var context = new TdbContext(_options);
        Comment comment = new Comment()
        {
            Id = 10,
            Content = "a test comment",
        };
        var handler = new AddProjectCommentCommandHandler(context);
        Assert.That(async()=> await handler.Handle(new AddProjectCommentCommand(1, comment), CancellationToken.None),Throws.Exception.TypeOf<NotFoundException<Project>>());
    }
}