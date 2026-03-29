using AutoMapper;
using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Tasks;
using FlowDesk.Core.Entities;
using FlowDesk.Core.Enums;
using FlowDesk.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FlowDesk.API.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ILogger<TaskService> _logger;

    // Valid status transitions
    private static readonly Dictionary<TaskItemStatus, ISet<TaskItemStatus>> AllowedTransitions = new()
    {
        [TaskItemStatus.Todo] = new HashSet<TaskItemStatus> { TaskItemStatus.InProgress, TaskItemStatus.Cancelled },
        [TaskItemStatus.InProgress] = new HashSet<TaskItemStatus> { TaskItemStatus.Done, TaskItemStatus.Todo, TaskItemStatus.Cancelled },
        [TaskItemStatus.Done] = new HashSet<TaskItemStatus> { TaskItemStatus.InProgress },
        [TaskItemStatus.Cancelled] = new HashSet<TaskItemStatus> { TaskItemStatus.Todo },
    };

    public TaskService(ITaskRepository taskRepo, IProjectRepository projectRepo,
        UserManager<AppUser> userManager, IMapper mapper, ILogger<TaskService> logger)
    {
        _taskRepo = taskRepo;
        _projectRepo = projectRepo;
        _userManager = userManager;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<TaskResponseDto>> CreateTaskAsync(int projectId, CreateTaskDto dto, string userId)
    {
        if (!await _projectRepo.ExistsAsync(projectId))
            return ServiceResult<TaskResponseDto>.Failure("Project not found.", 404);

        if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.UtcNow.Date)
            return ServiceResult<TaskResponseDto>.Failure("Due date cannot be in the past.");

        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            AssigneeId = dto.AssigneeId,
            ProjectId = projectId,
            CreatedById = userId
        };

        var created = await _taskRepo.CreateAsync(task);
        var result = await _taskRepo.GetByIdAsync(created.Id);
        _logger.LogInformation("Task {TaskId} created in project {ProjectId} by user {UserId}", created.Id, projectId, userId);
        return ServiceResult<TaskResponseDto>.Success(_mapper.Map<TaskResponseDto>(result), 201);
    }

    public async Task<ServiceResult<PagedResult<TaskResponseDto>>> GetTasksByProjectAsync(int projectId, TaskFilterDto filter, string userId)
    {
        if (!await _projectRepo.ExistsAsync(projectId))
            return ServiceResult<PagedResult<TaskResponseDto>>.Failure("Project not found.", 404);

        var paged = await _taskRepo.GetByProjectIdAsync(projectId, filter);
        var mapped = new PagedResult<TaskResponseDto>
        {
            Items = _mapper.Map<IEnumerable<TaskResponseDto>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
        return ServiceResult<PagedResult<TaskResponseDto>>.Success(mapped);
    }

    public async Task<ServiceResult<TaskResponseDto>> GetTaskByIdAsync(int taskId, string userId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task is null)
            return ServiceResult<TaskResponseDto>.Failure("Task not found.", 404);

        return ServiceResult<TaskResponseDto>.Success(_mapper.Map<TaskResponseDto>(task));
    }

    public async Task<ServiceResult<TaskResponseDto>> UpdateTaskAsync(int taskId, UpdateTaskDto dto, string userId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task is null)
            return ServiceResult<TaskResponseDto>.Failure("Task not found.", 404);

        if (dto.Status.HasValue && dto.Status.Value != task.Status)
        {
            if (!AllowedTransitions[task.Status].Contains(dto.Status.Value))
                return ServiceResult<TaskResponseDto>.Failure(
                    $"Cannot transition from {task.Status} to {dto.Status.Value}.");
            task.Status = dto.Status.Value;
        }

        if (dto.Title is not null) task.Title = dto.Title;
        if (dto.Description is not null) task.Description = dto.Description;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        if (dto.DueDate.HasValue)
        {
            if (dto.DueDate.Value.Date < DateTime.UtcNow.Date)
                return ServiceResult<TaskResponseDto>.Failure("Due date cannot be in the past.");
            task.DueDate = dto.DueDate.Value;
        }
        if (dto.AssigneeId is not null) task.AssigneeId = dto.AssigneeId;

        var updated = await _taskRepo.UpdateAsync(task);
        return ServiceResult<TaskResponseDto>.Success(_mapper.Map<TaskResponseDto>(updated));
    }

    public async Task<ServiceResult<bool>> ArchiveTaskAsync(int taskId, string userId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task is null) return ServiceResult<bool>.Failure("Task not found.", 404);

        task.IsArchived = true;
        await _taskRepo.UpdateAsync(task);
        _logger.LogInformation("Task {TaskId} archived by user {UserId}", taskId, userId);
        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<bool>> DeleteTaskAsync(int taskId, string userId)
    {
        if (!await _taskRepo.ExistsAsync(taskId))
            return ServiceResult<bool>.Failure("Task not found.", 404);

        await _taskRepo.DeleteAsync(taskId);
        return ServiceResult<bool>.Success(true);
    }
}