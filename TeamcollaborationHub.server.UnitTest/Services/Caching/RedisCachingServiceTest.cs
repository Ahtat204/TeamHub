using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;
using TeamcollborationHub.server.Services.Caching;

namespace TeamcollaborationHub.server.UnitTest.Services.Caching;

[TestFixture]
[TestOf(typeof(RedisCachingService))]
public class RedisCachingServiceTest
{
    private Project? Project;

    private IDistributedCache _cache;
    RedisCachingService _redisCachingService;

    [SetUp]
    public void Setup()
    {
        _cache = new MemoryDistributedCache(
            new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        _redisCachingService = new RedisCachingService(_cache);
        Project = new Project
        {
            Id = 1,
            Name = "Test Project",
            Description = "This is a test project",
            status = ProjectStatus.Completed
        };
    }
    [Test]
    public void GetProjectFromCacheTest()
    {
        var id = 1;
        _redisCachingService.SetProjectInCache(id.ToString(), Project);
        var result = _redisCachingService.GetProjectFromCache(id);
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(Project.Id));
        Assert.That(result.Name, Is.EqualTo(Project.Name));
    }
}