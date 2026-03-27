using TeamcollborationHub.server.Features.Projects.Commands.SetProjectStartDate;

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
    public void SetProjectStartDateTest()
    {
        Assert.NotNull(_options);
        var context = new TdbContext(_options);
        Assert.NotNull(context);
    }
}