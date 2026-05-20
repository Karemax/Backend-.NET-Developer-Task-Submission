using Microsoft.AspNetCore.Mvc;
using MediatR;
using Project.Application.Authentication.Common;
using Project.Domain.Primitives;

namespace Project.API.Controllers;

[ApiController]
public abstract class ApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult ResultToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        var problemDetails = new ProblemDetails
        {
            Status = 400,
            Title = "Bad Request",
            Detail = result.Error.Description
        };
        return BadRequest(problemDetails);
    }
}
