using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Configuration;

public class TDBContext : DbContext
{
    public TDBContext(DbContextOptions<TDBContext> options) : base(options)
    {
    }
    DbSet<User> Users { get; set; }
    DbSet<Project> Projects { get; set; }
    DbSet<ProjectTask> Tasks { get; set; }

}
