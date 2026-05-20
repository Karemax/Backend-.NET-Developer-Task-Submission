using Project.Domain.Enums;
using Project.Domain.Primitives;

namespace Project.Domain.Entities;

public class User : Entity, IAuditableEntity
{
    private User(Guid id, string email, string passwordHash, string firstName, string lastName, Role role)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    // EF Core empty constructor
    #pragma warning disable CS8618
    private User() { }
    #pragma warning restore CS8618

    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Role Role { get; private set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public static User Create(Guid id, string email, string passwordHash, string firstName, string lastName, Role role)
    {
        return new User(id, email, passwordHash, firstName, lastName, role);
    }
}
