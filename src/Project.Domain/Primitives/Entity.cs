namespace Project.Domain.Primitives;

public abstract class Entity
{
    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; init; }
}
