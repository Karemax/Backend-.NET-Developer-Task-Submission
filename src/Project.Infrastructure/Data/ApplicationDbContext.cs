using Microsoft.EntityFrameworkCore;
using Project.Domain.Entities;
using Project.Domain.Primitives;
using ProjectEntity = Project.Domain.Entities.Project;

namespace Project.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditableEntities();
        return base.SaveChanges();
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Entity.CreatedAt = DateTimeOffset.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }
    }
}
