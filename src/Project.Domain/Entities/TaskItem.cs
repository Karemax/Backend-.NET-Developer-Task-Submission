using Project.Domain.Enums;
using Project.Domain.Primitives;

namespace Project.Domain.Entities;

public class TaskItem : Entity, IAuditableEntity
{
    private TaskItem(Guid id, Guid projectId, string title, string description, Status status, Priority priority, DateTimeOffset? dueDate, Guid? assigneeId)
        : base(id)
    {
        ProjectId = projectId;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        AssigneeId = assigneeId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    // EF Core empty constructor
    #pragma warning disable CS8618
    private TaskItem() { }
    #pragma warning restore CS8618

    public Guid ProjectId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Status Status { get; private set; }
    public Priority Priority { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public Guid? AssigneeId { get; private set; }

    // Navigation properties
    public Project Project { get; private set; } = null!;
    public User? Assignee { get; private set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public static TaskItem Create(
        Guid id,
        Guid projectId,
        string title,
        string description,
        Status status,
        Priority priority,
        DateTimeOffset? dueDate,
        Guid? assigneeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title cannot be empty.", nameof(title));
        }

        return new TaskItem(id, projectId, title, description, status, priority, dueDate, assigneeId);
    }

    public void Update(
        string title,
        string description,
        Status status,
        Priority priority,
        DateTimeOffset? dueDate,
        Guid? assigneeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title cannot be empty.", nameof(title));
        }

        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        AssigneeId = assigneeId;
    }

    public void UpdateStatus(Status status)
    {
        Status = status;
    }
}
