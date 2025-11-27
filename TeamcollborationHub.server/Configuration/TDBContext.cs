using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Configuration;

public class TDBContext : DbContext
{
  public  DbSet<User> Users { get; set; }
  public DbSet<Project> Projects { get; set; }
  public  DbSet<ProjectTask> Tasks { get; set; }
  public  DbSet<Comment> Comments { get; set; }
    public TDBContext(DbContextOptions<TDBContext> options) : base(options)
    {
    }
  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Project>().HasMany(p => p.contributor).WithOne(u => u.project).HasForeignKey(u => u.projectId);
        modelBuilder.Entity<Project>().HasMany(p => p.Tasks).WithOne(t => t.project).HasForeignKey(t => t.projectId);
        modelBuilder.Entity<Project>().HasMany(p => p.comments).WithOne(c => c.Project).HasForeignKey(c => c.projectId);
        modelBuilder.Entity<Project>().Property(o=>o.status).HasConversion<string>();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}
