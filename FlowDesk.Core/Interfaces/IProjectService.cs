using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Projects;

namespace FlowDesk.Core.Interfaces;

public interface IProjectService
{
    Task<ServiceResult<ProjectResponseDto>> CreateProjectAsync(CreateProjectDto dto, string userId);
    Task<ServiceResult<IEnumerable<ProjectResponseDto>>> GetAllProjectsAsync(string userId);
    Task<ServiceResult<ProjectResponseDto>> GetProjectByIdAsync(int id, string userId);
    Task<ServiceResult<ProjectResponseDto>> UpdateProjectAsync(int id, CreateProjectDto dto, string userId);
    Task<ServiceResult<bool>> ArchiveProjectAsync(int id, string userId);
}