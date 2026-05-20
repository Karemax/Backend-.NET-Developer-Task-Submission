using System;

namespace Project.Application.Common.Contracts;

public record ProjectResponse(
    Guid Id,
    string Name,
    string Description,
    Guid OwnerId,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public record CreateProjectRequest(string Name, string Description);
public record UpdateProjectRequest(string Name, string Description);
