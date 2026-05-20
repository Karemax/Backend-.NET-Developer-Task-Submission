namespace Project.Application.Authentication.Common;

public record AuthResponse(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Token);
