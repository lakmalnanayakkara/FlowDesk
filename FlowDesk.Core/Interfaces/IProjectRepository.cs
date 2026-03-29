using FlowDesk.Core.Entities;

namespace FlowDesk.Core.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id);
    Task<IEnumerable<Project>> GetAllAsync(string userId, bool includeArchived = false);
    Task<Project> CreateAsync(Project project);
    Task<Project> UpdateAsync(Project project);
    Task<bool> ExistsAsync(int id);
}