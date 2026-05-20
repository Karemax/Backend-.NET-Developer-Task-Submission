using MediatR;
using Project.Application.Abstractions.Data;
using Project.Application.Authentication.Common;
using Project.Domain.Entities;
using Project.Domain.Enums;
using Project.Domain.Primitives;
using System;

namespace Project.Application.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateTimeOffset? DueDate,
    Guid? AssigneeId,
    Guid UserId) : IRequest<Result<Guid>>;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(
        ITaskItemRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        // Ensure the project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
            return Result.Failure<Guid>(Error.NullValue);
        
        if (project.OwnerId != request.UserId)
            return Result.Failure<Guid>(new Error("Error.Unauthorized", "User is not authorized to add tasks to this project."));

        if (request.AssigneeId.HasValue)
        {
            var assignee = await _userRepository.GetByIdAsync(request.AssigneeId.Value, cancellationToken);
            if (assignee is null)
                return Result.Failure<Guid>(new Error("Error.InvalidAssignee", "Assignee does not exist."));
        }

        var task = TaskItem.Create(
            Guid.NewGuid(),
            request.ProjectId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            request.AssigneeId);

        _taskRepository.Add(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(task.Id);
    }
}
