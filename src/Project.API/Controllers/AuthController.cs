using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Authentication.Commands.Login;
using Project.Application.Authentication.Commands.Register;
using Project.Application.Authentication.Common;
using Project.Domain.Primitives;

namespace Project.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ApiController
{
    // private readonly IMapper _mapper; // optional if using mapping, not needed now

    public AuthController()
    {
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Created(string.Empty, result);
        }
        return ResultToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return ResultToActionResult(result);
    }
}
