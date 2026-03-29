using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FlowDesk.Core.DTOs.Auth;
using FlowDesk.Core.DTOs.Projects;
using FlowDesk.Core.DTOs.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlowDesk.Tests.Integration;

public class TasksControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TasksControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetTokenAsync()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginDto { Email = "admin@flowdesk.com", Password = "Admin@123" });
        var auth = await login.Content.ReadFromJsonAsync<AuthResponseDto>();
        return auth!.Token;
    }

    [Fact]
    public async Task CreateTask_WithValidData_Returns201()
    {
        var token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Create a project first
        var projectRes = await _client.PostAsJsonAsync("/api/projects",
            new CreateProjectDto { Name = "Test Project" });
        var project = await projectRes.Content.ReadFromJsonAsync<ProjectResponseDto>();

        var dto = new CreateTaskDto
        {
            Title = "Integration Test Task",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var response = await _client.PostAsJsonAsync($"/api/projects/{project!.Id}/tasks", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/projects/1/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}