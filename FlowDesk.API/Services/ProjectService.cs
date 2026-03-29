using AutoMapper;
using FlowDesk.Core.Common;
using FlowDesk.Core.DTOs.Projects;
using FlowDesk.Core.Entities;
using FlowDesk.Core.Interfaces;

namespace FlowDesk.API.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IProjectRepository projectRepo, IMapper mapper, ILogger<ProjectService> logger)
    {
        _projectRepo = projectRepo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<ProjectResponseDto>> CreateProjectAsync(CreateProjectDto dto, string userId)
    {
        var project = new Project { Name = dto.Name, Description = dto.Description, OwnerId = userId };
        var created = await _projectRepo.CreateAsync(project);
        var fetched = await _projectRepo.GetByIdAsync(created.Id);
        _logger.LogInformation("Project {ProjectId} created by user {UserId}", created.Id, userId);
        return ServiceResult<ProjectResponseDto>.Success(_mapper.Map<ProjectResponseDto>(fetched!), 201);
    }

    public async Task<ServiceResult<IEnumerable<ProjectResponseDto>>> GetAllProjectsAsync(string userId)
    {
        var projects = await _projectRepo.GetAllAsync(userId);
        return ServiceResult<IEnumerable<ProjectResponseDto>>.Success(_mapper.Map<IEnumerable<ProjectResponseDto>>(projects));
    }

    public async Task<ServiceResult<ProjectResponseDto>> GetProjectByIdAsync(int id, string userId)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project is null || project.OwnerId != userId)
            return ServiceResult<ProjectResponseDto>.Failure("Project not found.", 404);

        return ServiceResult<ProjectResponseDto>.Success(_mapper.Map<ProjectResponseDto>(project));
    }

    public async Task<ServiceResult<ProjectResponseDto>> UpdateProjectAsync(int id, CreateProjectDto dto, string userId)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project is null || project.OwnerId != userId)
            return ServiceResult<ProjectResponseDto>.Failure("Project not found.", 404);

        project.Name = dto.Name;
        project.Description = dto.Description;
        var updated = await _projectRepo.UpdateAsync(project);
        return ServiceResult<ProjectResponseDto>.Success(_mapper.Map<ProjectResponseDto>(updated));
    }

    public async Task<ServiceResult<bool>> ArchiveProjectAsync(int id, string userId)
    {
        var project = await _projectRepo.GetByIdAsync(id);
        if (project is null || project.OwnerId != userId)
            return ServiceResult<bool>.Failure("Project not found.", 404);

        project.IsArchived = true;
        await _projectRepo.UpdateAsync(project);
        return ServiceResult<bool>.Success(true);
    }
}