using Project.Application.Projects.Commands.CreateProject;
using Project.Application.Projects.Queries.GetProjectsPaged;
using Project.Application.Abstractions.Data;
using Project.Domain.Entities;
using Project.Domain.Primitives;
using ProjectEntity = Project.Domain.Entities.Project;

namespace Project.UnitTests;

public class UnitTest1
{
    [Fact]
    public async Task CreateProjectCommandHandler_ShouldReturnNewProjectId()
    {
        var repository = new InMemoryProjectRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateProjectCommandHandler(repository, unitOfWork);

        var command = new CreateProjectCommand("Test Project", "Test description", Guid.NewGuid());
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        Assert.Single(repository.Projects);
    }

    [Fact]
    public async Task GetProjectsPagedQueryHandler_ShouldReturnMappedProjectResponses()
    {
        var userId = Guid.NewGuid();
        var repository = new InMemoryProjectRepository();
        repository.Projects.Add(ProjectEntity.Create(Guid.NewGuid(), "Project A", "Description", userId));
        repository.Projects.Add(ProjectEntity.Create(Guid.NewGuid(), "Project B", "Description", Guid.NewGuid()));

        var handler = new GetProjectsPagedQueryHandler(repository);
        var query = new GetProjectsPagedQuery(userId, 1, 10);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Project A", result.Value[0].Name);
    }
}

internal class InMemoryProjectRepository : IProjectRepository
{
    public List<ProjectEntity> Projects { get; } = new();

    public void Add(ProjectEntity project)
    {
        Projects.Add(project);
    }

    public Task<ProjectEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Projects.FirstOrDefault(p => p.Id == id));
    }

    public Task<ProjectEntity?> GetByIdWithOwnerAndTasksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Projects.FirstOrDefault(p => p.Id == id));
    }

    public Task<List<ProjectEntity>> GetPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var items = Projects.Where(p => p.OwnerId == userId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Task.FromResult(items);
    }

    public void Remove(ProjectEntity project)
    {
        Projects.Remove(project);
    }

    public void Update(ProjectEntity project)
    {
        var index = Projects.FindIndex(p => p.Id == project.Id);
        if (index >= 0)
        {
            Projects[index] = project;
        }
    }
}

internal class FakeUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }
}
