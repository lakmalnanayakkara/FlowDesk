using FlowDesk.Core.Enums;

namespace FlowDesk.Core.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsArchived { get; set; } = false;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string? AssigneeId { get; set; }
    public AppUser? Assignee { get; set; }

    public string CreatedById { get; set; } = string.Empty;
    public AppUser CreatedBy { get; set; } = null!;
}