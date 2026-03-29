using System.Security.Claims;
using FlowDesk.Core.DTOs.Projects;
using FlowDesk.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public ProjectsController(IProjectService projectService) => _projectService = projectService;

    /// <summary>Get all projects owned by the current user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _projectService.GetAllProjectsAsync(UserId);
        return Ok(result.Data);
    }

    /// <summary>Get a specific project by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _projectService.GetProjectByIdAsync(id, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return Ok(result.Data);
    }

    /// <summary>Create a new project.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var result = await _projectService.CreateProjectAsync(dto, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return StatusCode(201, result.Data);
    }

    /// <summary>Update an existing project.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProjectDto dto)
    {
        var result = await _projectService.UpdateProjectAsync(id, dto, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return Ok(result.Data);
    }

    /// <summary>Archive a project (soft delete).</summary>
    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var result = await _projectService.ArchiveProjectAsync(id, UserId);
        if (!result.IsSuccess) return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
        return Ok(new { message = "Project archived successfully." });
    }
}