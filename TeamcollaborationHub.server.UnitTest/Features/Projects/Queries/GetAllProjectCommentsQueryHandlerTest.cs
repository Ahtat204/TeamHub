using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectComments;

namespace TeamcollaborationHub.server.UnitTest.Features.Projects.Queries;

[TestFixture]
[TestOf(typeof(GetAllProjectCommentsQueryHandler))]
public class GetAllProjectCommentsQueryHandlerTest
{
    private DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }

    [Test]
    public async Task GetAllProjectCommentsTest()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        Comment com = new Comment()
        {
            Id = 1, Content = "this is content", projectId = project.Id
        };
        context.Comments.Add(com);
        await context.SaveChangesAsync();
        var handler = new GetAllProjectCommentsQueryHandler(context);
        var result = await handler.Handle(new GetAllProjectCommentsQuery(project.Id), CancellationToken.None);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstOrDefault().Content, Is.Not.Null);
        Assert.That("this is content", Is.EqualTo(result.FirstOrDefault()?.Content));
        await context.Database.EnsureDeletedAsync();
    }

    [Test]
    public async Task GetAllProjectCommentsTest_ShouldReturnEmptyList()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        await context.SaveChangesAsync();
        var handler = new GetAllProjectCommentsQueryHandler(context);
        var result = await handler.Handle(new GetAllProjectCommentsQuery(project.Id), CancellationToken.None);
        Assert.NotNull(result);
        Assert.That(result.Count, Is.EqualTo(0));
        await context.Database.EnsureDeletedAsync();
    }

    [Test]
    public async Task GetAllProjectCommentsTest_ShouldThrowNotFoundException()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var project = new Project { Id = 1, Name = "Project 1" };
        var handler = new GetAllProjectCommentsQueryHandler(context);
        Assert.That(() => handler.Handle(new GetAllProjectCommentsQuery(project.Id), CancellationToken.None),
            Throws.Exception.TypeOf<NotFoundException<Project>>());
        await context.Database.EnsureDeletedAsync();
    }
}