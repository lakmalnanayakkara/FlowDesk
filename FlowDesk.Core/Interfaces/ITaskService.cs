using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Tasks;

namespace FlowDesk.Core.Interfaces;

public interface ITaskService
{
    Task<ServiceResult<TaskResponseDto>> CreateTaskAsync(int projectId, CreateTaskDto dto, string userId);
    Task<ServiceResult<PagedResult<TaskResponseDto>>> GetTasksByProjectAsync(int projectId, TaskFilterDto filter, string userId);
    Task<ServiceResult<TaskResponseDto>> GetTaskByIdAsync(int taskId, string userId);
    Task<ServiceResult<TaskResponseDto>> UpdateTaskAsync(int taskId, UpdateTaskDto dto, string userId);
    Task<ServiceResult<bool>> ArchiveTaskAsync(int taskId, string userId);
    Task<ServiceResult<bool>> DeleteTaskAsync(int taskId, string userId);
}