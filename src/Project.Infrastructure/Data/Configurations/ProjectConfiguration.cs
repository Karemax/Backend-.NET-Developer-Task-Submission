using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectEntity = Project.Domain.Entities.Project;

namespace Project.Infrastructure.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<ProjectEntity>
{
    public void Configure(EntityTypeBuilder<ProjectEntity> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.OwnerId)
            .IsRequired();

        // One-to-many relationship with Owner (User)
        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-many relationship with TaskItems
        // Note: TaskItem configuration will handle the relationship config or we can do it here.
        builder.HasMany(p => p.Tasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
