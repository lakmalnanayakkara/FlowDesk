using FlowDesk.Core.Enums;

namespace FlowDesk.Core.DTOs.Tasks;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? AssigneeId { get; set; }
}