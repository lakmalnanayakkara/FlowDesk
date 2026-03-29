using Microsoft.AspNetCore.Identity;

namespace FlowDesk.Core.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
}