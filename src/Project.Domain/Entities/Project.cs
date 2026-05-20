using Project.Domain.Primitives;

namespace Project.Domain.Entities;

public class Project : Entity, IAuditableEntity
{
    private readonly List<TaskItem> _tasks = new();

    private Project(Guid id, string name, string description, Guid ownerId)
        : base(id)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    // EF Core empty constructor
    #pragma warning disable CS8618
    private Project() { }
    #pragma warning restore CS8618

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid OwnerId { get; private set; }

    // Navigation properties
    public User Owner { get; private set; } = null!;
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public static Project Create(Guid id, string name, string description, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Project name cannot be empty.", nameof(name));
        }

        return new Project(id, name, description, ownerId);
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Project name cannot be empty.", nameof(name));
        }

        Name = name;
        Description = description;
    }
}
