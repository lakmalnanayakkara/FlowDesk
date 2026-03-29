using AutoMapper;
using FlowDesk.Core.DTOs.Projects;
using FlowDesk.Core.DTOs.Tasks;
using FlowDesk.Core.Entities;

namespace FlowDesk.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TaskItem, TaskResponseDto>()
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Priority, o => o.MapFrom(s => s.Priority.ToString()))
            .ForMember(d => d.AssigneeName, o => o.MapFrom(s => s.Assignee != null ? s.Assignee.FullName : null))
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => s.CreatedBy.FullName));

        CreateMap<Project, ProjectResponseDto>()
            .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.Owner.FullName))
            .ForMember(d => d.TaskCount, o => o.MapFrom(s => s.Tasks.Count(t => !t.IsArchived)));
    }
}