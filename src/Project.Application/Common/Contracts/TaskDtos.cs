using System;
using Project.Domain.Enums;

namespace Project.Application.Common.Contracts;

public record TaskResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateTimeOffset? DueDate,
    Guid? AssigneeId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record CreateTaskRequest(
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateTimeOffset? DueDate,
    Guid? AssigneeId);

public record UpdateTaskRequest(
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateTimeOffset? DueDate,
    Guid? AssigneeId);
