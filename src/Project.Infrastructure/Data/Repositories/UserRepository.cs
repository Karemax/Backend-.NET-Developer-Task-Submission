using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Data;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }
}
