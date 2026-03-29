using FlowDesk.Core.Enums;

namespace FlowDesk.Core.DTOs.Tasks;

public class TaskFilterDto
{
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? AssigneeId { get; set; }
    public bool IncludeArchived { get; set; } = false;
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}