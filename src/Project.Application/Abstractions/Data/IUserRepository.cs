using Project.Domain.Entities;

namespace Project.Application.Abstractions.Data;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
    void Update(User user);
}
