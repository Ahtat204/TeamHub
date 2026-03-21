using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Configuration;

public sealed class TdbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public TdbContext(DbContextOptions<TdbContext> options) : base(options)
    {
        if (Database.GetService<IDatabaseCreator>() is not RelationalDatabaseCreator databasecreator) return;
        if (!databasecreator.Exists() || !databasecreator.CanConnect()) databasecreator.Create();
        if (!databasecreator.HasTables()) databasecreator.CreateTables();
    }

    public TdbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Project>().HasMany(p => p.Contributors).WithOne(u => u.project)
            .HasForeignKey(u => u.ProjectId);
        modelBuilder.Entity<Project>().HasMany(p => p.Tasks).WithOne(t => t.project).HasForeignKey(t => t.projectId);
        modelBuilder.Entity<Project>().HasMany(p => p.Comments).WithOne(c => c.Project).HasForeignKey(c => c.projectId);
        modelBuilder.Entity<Project>().Property(o => o.Status).HasConversion<string>();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }
}