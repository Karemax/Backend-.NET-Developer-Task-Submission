namespace Project.Domain.Primitives;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? UpdatedAt { get; set; }
}
