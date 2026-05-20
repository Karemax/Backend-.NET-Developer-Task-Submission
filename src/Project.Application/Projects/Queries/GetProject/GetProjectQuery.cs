using System;
using Mapster;
using MediatR;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Application.Common.Contracts;
using Project.Domain.Primitives;

namespace Project.Application.Projects.Queries.GetProject;

public record GetProjectQuery(Guid ProjectId) : IRequest<Result<ProjectResponse>>;

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, Result<ProjectResponse>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICacheService _cacheService;

    public GetProjectQueryHandler(IProjectRepository projectRepository, ICacheService cacheService)
    {
        _projectRepository = projectRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<ProjectResponse>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"project:{request.ProjectId}";
        var cached = await _cacheService.GetAsync<ProjectResponse>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<ProjectResponse>(Error.NullValue);
        }

        var response = project.Adapt<ProjectResponse>();
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);
        return Result.Success(response);
    }
}
