using System;
using Mapster;
using MediatR;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Application.Common.Contracts;
using Project.Domain.Entities;
using Project.Domain.Primitives;
using Project.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Application.Tasks.Queries.GetTasksPaged;

public record GetTasksPagedQuery(Guid ProjectId,
        int PageNumber,
        int PageSize,
        Status? Status,
        Priority? Priority) : IRequest<Result<List<TaskResponse>>>;

public class GetTasksPagedQueryHandler : IRequestHandler<GetTasksPagedQuery, Result<List<TaskResponse>>>
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly ICacheService _cacheService;

    public GetTasksPagedQueryHandler(ITaskItemRepository taskRepository, ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<List<TaskResponse>>> Handle(GetTasksPagedQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"project:{request.ProjectId}:tasks:{request.PageNumber}:{request.PageSize}:{request.Status}:{request.Priority}";
        var cached = await _cacheService.GetAsync<List<TaskResponse>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var tasks = await _taskRepository.GetPagedByProjectIdAsync(request.ProjectId, request.PageNumber, request.PageSize, request.Status, request.Priority, cancellationToken);
        var response = tasks.Adapt<List<TaskResponse>>();
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);
        return Result.Success(response);
    }
}
