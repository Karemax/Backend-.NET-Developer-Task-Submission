using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace Project.API.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title, errors) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                ExtractValidationErrors(validationException)),
            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                null),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                null),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                null)
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static Dictionary<string, string[]> ExtractValidationErrors(ValidationException exception)
    {
        return exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
    }
}
