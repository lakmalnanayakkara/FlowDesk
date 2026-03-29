using System.Security.Claims;
using FlowDesk.Core.DTOs.Tasks;
using FlowDesk.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowDesk.API.Controllers;

[ApiController]
[Route("api/projects/{projectId:int}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    /// <summary>Get all tasks in a project, with filtering and pagination.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(int projectId, [FromQuery] TaskFilterDto filter)
    {
        var result = await _taskService.GetTasksByProjectAsync(projectId, filter, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return Ok(result.Data);
    }

    /// <summary>Get a single task by ID.</summary>
    [HttpGet("{taskId:int}")]
    public async Task<IActionResult> GetById(int projectId, int taskId)
    {
        var result = await _taskService.GetTaskByIdAsync(taskId, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return Ok(result.Data);
    }

    /// <summary>Create a new task in a project.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(int projectId, [FromBody] CreateTaskDto dto)
    {
        var result = await _taskService.CreateTaskAsync(projectId, dto, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return StatusCode(201, result.Data);
    }

    /// <summary>Update a task's details or status.</summary>
    [HttpPut("{taskId:int}")]
    public async Task<IActionResult> Update(int projectId, int taskId, [FromBody] UpdateTaskDto dto)
    {
        var result = await _taskService.UpdateTaskAsync(taskId, dto, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return Ok(result.Data);
    }

    /// <summary>Archive a task without permanently deleting it.</summary>
    [HttpPost("{taskId:int}/archive")]
    public async Task<IActionResult> Archive(int projectId, int taskId)
    {
        var result = await _taskService.ArchiveTaskAsync(taskId, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return Ok(new { message = "Task archived successfully." });
    }

    /// <summary>Permanently delete a task.</summary>
    [HttpDelete("{taskId:int}")]
    public async Task<IActionResult> Delete(int projectId, int taskId)
    {
        var result = await _taskService.DeleteTaskAsync(taskId, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return NoContent();
    }
}