using AutoMapper;
using FlowDesk.API.Services;
using FlowDesk.Core.DTOs.Tasks;
using FlowDesk.Core.Entities;
using FlowDesk.Core.Enums;
using FlowDesk.Core.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlowDesk.Tests.Unit;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<TaskService>> _loggerMock = new();
    private readonly Mock<UserManager<AppUser>> _userManagerMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _sut = new TaskService(_taskRepoMock.Object, _projectRepoMock.Object,
            _userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_WhenProjectNotFound_ReturnsFailure()
    {
        _projectRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        var result = await _sut.CreateTaskAsync(1, new CreateTaskDto { Title = "Test" }, "user-1");

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateTaskAsync_WhenDueDateInPast_ReturnsFailure()
    {
        _projectRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

        var dto = new CreateTaskDto { Title = "Test", DueDate = DateTime.UtcNow.AddDays(-1) };
        var result = await _sut.CreateTaskAsync(1, dto, "user-1");

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("past");
    }

    [Fact]
    public async Task UpdateTaskAsync_InvalidStatusTransition_ReturnsFailure()
    {
        var task = new TaskItem { Id = 1, Title = "Test", Status = TaskItemStatus.Todo, CreatedBy = new AppUser() };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var dto = new UpdateTaskDto { Status = TaskItemStatus.Done }; // Todo -> Done is invalid
        var result = await _sut.UpdateTaskAsync(1, dto, "user-1");

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Cannot transition");
    }

    [Fact]
    public async Task UpdateTaskAsync_ValidStatusTransition_ReturnsSuccess()
    {
        var task = new TaskItem { Id = 1, Title = "Test", Status = TaskItemStatus.Todo, CreatedBy = new AppUser() };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(task);
        _mapperMock.Setup(m => m.Map<TaskResponseDto>(It.IsAny<TaskItem>())).Returns(new TaskResponseDto());

        var dto = new UpdateTaskDto { Status = TaskItemStatus.InProgress }; // Todo -> InProgress is valid
        var result = await _sut.UpdateTaskAsync(1, dto, "user-1");

        result.IsSuccess.Should().BeTrue();
    }
}