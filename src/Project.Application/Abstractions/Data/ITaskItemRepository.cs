using Project.Domain.Entities;
using Project.Domain.Enums;

namespace Project.Application.Abstractions.Data;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TaskItem>> GetPagedByProjectIdAsync(
        Guid projectId,
        int page,
        int pageSize,
        Status? status,
        Priority? priority,
        CancellationToken cancellationToken = default);
    void Add(TaskItem taskItem);
    void Update(TaskItem taskItem);
    void Remove(TaskItem taskItem);
}
