using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Tasks;
using FlowDesk.Core.Entities;
using FlowDesk.Core.Interfaces;
using FlowDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context) => _context = context;

    public async Task<TaskItem?> GetByIdAsync(int id) =>
        await _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.CreatedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<PagedResult<TaskItem>> GetByProjectIdAsync(int projectId, TaskFilterDto filter)
    {
        var query = _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.CreatedBy)
            .Where(t => t.ProjectId == projectId);

        if (!filter.IncludeArchived)
            query = query.Where(t => !t.IsArchived);

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status);

        if (filter.Priority.HasValue)
            query = query.Where(t => t.Priority == filter.Priority);

        if (!string.IsNullOrEmpty(filter.AssigneeId))
            query = query.Where(t => t.AssigneeId == filter.AssigneeId);

        query = filter.SortBy?.ToLower() switch
        {
            "duedate" => filter.SortDirection == "asc" ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
            "priority" => filter.SortDirection == "asc" ? query.OrderBy(t => t.Priority) : query.OrderByDescending(t => t.Priority),
            "status" => filter.SortDirection == "asc" ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status),
            _ => filter.SortDirection == "asc" ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();

        return new PagedResult<TaskItem>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        task.UpdatedAt = DateTime.UtcNow;
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null) return false;
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Tasks.AnyAsync(t => t.Id == id);
}