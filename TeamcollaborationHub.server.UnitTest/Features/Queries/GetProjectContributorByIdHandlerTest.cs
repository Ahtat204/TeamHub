using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectContributors;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;

namespace TeamcollaborationHub.server.UnitTest.Features.Queries;

[TestFixture]

public class GetProjectContributorByIdHandlerTest
{
    DbContextOptions<TdbContext>? _options;

    [SetUp]
    public void Setup()
    {
        _options=new DbContextOptionsBuilder<TdbContext>().UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
       
    }
    
    [Test]
    public void GetProjectContributorById_ShouldReturnContributor()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var project = new Project { Id = 1, Name = "Project 1" };
        context.Projects.Add(project);
        var contributor = new User { Id = 1, Name = "Contributor 1", ProjectId = project.Id };
        context.Users.Add(contributor);
        context.SaveChanges();
        var handler = new GetAllProjectContributorsQueryHandler(context);
        var result = handler.Handle(new GetAllProjectContributorsQuery(project.Id), CancellationToken.None).Result;
        Assert.IsNotNull(result);
        Assert.That(result.Count(), Is.EqualTo(1));
        var projectResult = result.First();
        Assert.That(projectResult.Id, Is.EqualTo(project.Id));
        Assert.That(projectResult.Name, Is.EqualTo(contributor.Name));
    }

    [Test]
    public void GetProjectContributorId_ShouldReturnNull()
    {
        Assert.NotNull(_options);
        using var context = new TdbContext(_options);
        var project = new Project { Id = 2, Name = "Project 1" };
        context.Projects.Add(project);
        context.SaveChanges();
        var handler=new GetAllProjectContributorsQueryHandler(context);
        var result= handler.Handle(new GetAllProjectContributorsQuery(project.Id), CancellationToken.None).Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
        
    }
}
