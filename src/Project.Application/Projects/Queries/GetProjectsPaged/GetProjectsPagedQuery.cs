using Mapster;
using MediatR;
using Project.Application.Abstractions.Data;
using Project.Application.Common.Contracts;
using Project.Domain.Primitives;

namespace Project.Application.Projects.Queries.GetProjectsPaged;

public record GetProjectsPagedQuery(Guid UserId, int Page, int PageSize) : IRequest<Result<List<ProjectResponse>>>;

public class GetProjectsPagedQueryHandler : IRequestHandler<GetProjectsPagedQuery, Result<List<ProjectResponse>>>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsPagedQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result<List<ProjectResponse>>> Handle(GetProjectsPagedQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetPagedAsync(request.UserId, request.Page, request.PageSize, cancellationToken);
        return Result.Success(projects.Adapt<List<ProjectResponse>>());
    }
}
