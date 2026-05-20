using ProjectEntity = Project.Domain.Entities.Project;

namespace Project.Application.Abstractions.Data;

public interface IProjectRepository
{
    Task<ProjectEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProjectEntity?> GetByIdWithOwnerAndTasksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProjectEntity>> GetPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    void Add(ProjectEntity project);
    void Update(ProjectEntity project);
    void Remove(ProjectEntity project);
}
