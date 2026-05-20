using System;
using MediatR;
using Project.Application.Abstractions.Caching;
using Project.Application.Abstractions.Data;
using Project.Domain.Enums;
using Project.Domain.Primitives;

namespace Project.Application.Tasks.Commands.UpdateTask;


public record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateTimeOffset? DueDate,
    Guid? AssigneeId,
    Guid UserId) : IRequest<Result>;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result>
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public UpdateTaskCommandHandler(ITaskItemRepository taskRepository, IProjectRepository projectRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Failure(Error.NullValue);
        }

        var project = await _projectRepository.GetByIdAsync(task.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure(new Error("Error.NullValue", "Parent project not found."));

        if (project.OwnerId != request.UserId)
            return Result.Failure(new Error("Error.Unauthorized", "User is not authorized to modify this task."));

        if (request.AssigneeId.HasValue)
        {
            var assignee = await _userRepository.GetByIdAsync(request.AssigneeId.Value, cancellationToken);
            if (assignee is null)
                return Result.Failure(new Error("Error.InvalidAssignee", "Assignee does not exist."));
        }

        task.Update(
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            request.AssigneeId);

        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveAsync($"task:{request.TaskId}", cancellationToken);
        return Result.Success();
    }
}
