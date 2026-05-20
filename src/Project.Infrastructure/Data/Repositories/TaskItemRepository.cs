using Microsoft.EntityFrameworkCore;
using Project.Application.Abstractions.Data;
using Project.Domain.Entities;
using Project.Domain.Enums;

namespace Project.Infrastructure.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TaskItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaskItems
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<TaskItem>> GetPagedByProjectIdAsync(
        Guid projectId,
        int page,
        int pageSize,
        Status? status,
        Priority? priority,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TaskItems
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public void Add(TaskItem taskItem)
    {
        _dbContext.TaskItems.Add(taskItem);
    }

    public void Update(TaskItem taskItem)
    {
        _dbContext.TaskItems.Update(taskItem);
    }

    public void Remove(TaskItem taskItem)
    {
        _dbContext.TaskItems.Remove(taskItem);
    }
}
