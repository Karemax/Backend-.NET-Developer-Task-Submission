using System;
using MediatR;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Domain.Primitives;

namespace Project.Application.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId, Guid UserId) : IRequest<Result>;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public DeleteTaskCommandHandler(ITaskItemRepository taskRepository, IProjectRepository projectRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
            return Result.Failure(Error.NullValue);
        var project = await _projectRepository.GetByIdAsync(task.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(new Error("Error.NullValue", "Parent project not found."));

        if (project.OwnerId != request.UserId)
            return Result.Failure(new Error("Error.Unauthorized", "User is not authorized to delete this task."));

        _taskRepository.Remove(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"task:{request.TaskId}", cancellationToken);
        return Result.Success();
    }
}
