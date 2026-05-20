using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Common.Contracts;
using Project.API.Extensions;
using Project.Application.Tasks.Commands.CreateTask;
using Project.Application.Tasks.Commands.UpdateTask;
using Project.Application.Tasks.Commands.DeleteTask;
using Project.Application.Tasks.Queries.GetTask;
using Project.Application.Tasks.Queries.GetTasksPaged;
using Project.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.API.Controllers.Projects;

[ApiController]
[Authorize]
[Route("api/v1/projects/{projectId:guid}/tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Result<Guid>>> CreateTask(Guid projectId, [FromBody] CreateTaskRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var command = new CreateTaskCommand(projectId, request.Title, request.Description, request.Status, request.Priority, request.DueDate, request.AssigneeId, userId.Value);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetTask), new { projectId, taskId = result.Value }, result);
        return BadRequest(result);
    }

    [HttpGet("{taskId:guid}")]
    public async Task<ActionResult<Result<TaskResponse>>> GetTask(Guid projectId, Guid taskId)
    {
        var query = new GetTaskQuery(taskId);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
            return Ok(result);
        return NotFound(result);
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<TaskResponse>>>> GetPaged(Guid projectId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetTasksPagedQuery(projectId, pageNumber, pageSize, null, null);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<ActionResult<Result>> UpdateTask(Guid projectId, Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var updateCommand = new UpdateTaskCommand(taskId, request.Title, request.Description, request.Status, request.Priority, request.DueDate, request.AssigneeId, userId.Value);
        var result = await _mediator.Send(updateCommand);
        if (result.IsSuccess)
            return NoContent();
        return BadRequest(result);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<ActionResult<Result>> DeleteTask(Guid projectId, Guid taskId)
    {
        var userId = User.GetUserId();
        if (userId is null)
            return Unauthorized();

        var command = new DeleteTaskCommand(taskId, userId.Value);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return NoContent();
        return BadRequest(result);
    }
}
