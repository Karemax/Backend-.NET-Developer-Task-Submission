using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.API.Extensions;
using Project.Application.Common.Contracts;
using Project.Application.Projects.Commands.CreateProject;
using Project.Application.Projects.Commands.UpdateProject;
using Project.Application.Projects.Commands.DeleteProject;
using Project.Application.Projects.Queries.GetProject;
using Project.Application.Projects.Queries.GetProjectsPaged;
using Project.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreateProjectRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var command = new CreateProjectCommand(request.Name, request.Description, userId.Value);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result);
        return BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Result<ProjectResponse>>> GetById(Guid id)
    {
        var query = new GetProjectQuery(id);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
            return Ok(result);
        return NotFound(result);
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<ProjectResponse>>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var query = new GetProjectsPagedQuery(userId.Value, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Result>> Update(Guid id, [FromBody] UpdateProjectRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var updateCommand = new UpdateProjectCommand(id, request.Name, request.Description, userId.Value);
        var result = await _mediator.Send(updateCommand);
        if (result.IsSuccess)
            return NoContent();
        return BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Result>> Delete(Guid id)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var command = new DeleteProjectCommand(id, userId.Value);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return NoContent();
        return BadRequest(result);
    }
}
