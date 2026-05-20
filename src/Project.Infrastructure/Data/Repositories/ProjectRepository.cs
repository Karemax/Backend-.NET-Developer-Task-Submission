using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Data;
using ProjectEntity = Project.Domain.Entities.Project;

namespace Project.Infrastructure.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProjectRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProjectEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ProjectEntity?> GetByIdWithOwnerAndTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .Include(p => p.Owner)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<ProjectEntity>> GetPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public void Add(ProjectEntity project)
    {
        _dbContext.Projects.Add(project);
    }

    public void Update(ProjectEntity project)
    {
        _dbContext.Projects.Update(project);
    }

    public void Remove(ProjectEntity project)
    {
        _dbContext.Projects.Remove(project);
    }
}
