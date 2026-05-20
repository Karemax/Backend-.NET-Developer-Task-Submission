using System;
using Mapster;
using MediatR;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Application.Common.Contracts;
using Project.Domain.Primitives;
using Project.Domain.Errors;

namespace Project.Application.Tasks.Queries.GetTask;

public record GetTaskQuery(Guid TaskId) : IRequest<Result<TaskResponse>>;

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, Result<TaskResponse>>
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly ICacheService _cacheService;

    public GetTaskQueryHandler(ITaskItemRepository taskRepository, ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<TaskResponse>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"task:{request.TaskId}";
        var cached = await _cacheService.GetAsync<TaskResponse>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Failure<TaskResponse>(Error.NullValue);
        }

        var response = task.Adapt<TaskResponse>();
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);
        return Result.Success(response);
    }
}
