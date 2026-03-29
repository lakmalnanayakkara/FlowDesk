using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Tasks;
using FlowDesk.Core.Entities;

namespace FlowDesk.Core.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int id);
    Task<PagedResult<TaskItem>> GetByProjectIdAsync(int projectId, TaskFilterDto filter);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}