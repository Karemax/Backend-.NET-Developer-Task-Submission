using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("TaskItems");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.DueDate);

        builder.Property(t => t.AssigneeId);

        // Relationship with Project is configured in ProjectConfiguration
        // but we can also set the foreign key here to be explicit
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-many relationship with Assignee (User)
        builder.HasOne(t => t.Assignee)
            .WithMany()
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
