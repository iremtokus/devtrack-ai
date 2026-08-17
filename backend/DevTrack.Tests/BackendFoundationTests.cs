using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DevTrack.Application;
using DevTrack.Domain;
using DevTrack.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DevTrack.Tests;

public class TestApiFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"devtrack-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); builder.UseSetting("ConnectionStrings:DevTrack", "Host=unused;Database=unused"); builder.UseSetting("Jwt:Key", "test-only-jwt-signing-key-at-least-32-bytes"); builder.UseSetting("Cors:AllowedOrigins:0", "http://localhost:5173");
        builder.ConfigureServices(services => { services.RemoveAll<DbContextOptions<DevTrackDbContext>>(); services.AddDbContext<DevTrackDbContext>(o => o.UseInMemoryDatabase(databaseName)); });
    }
}

public class InvalidAiFactory : TestApiFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services => { services.RemoveAll<IAiLearningService>(); services.AddSingleton<IAiLearningService, InvalidReferenceAiService>(); });
    }
}

public class InvalidReferenceAiService : IAiLearningService
{
    public Task<AiProviderResult> SuggestAsync(AiLearningContext context, AiActionType action, CancellationToken cancellationToken) => Task.FromResult(new AiProviderResult("Invalid", "Invalid task reference", [], [new AiRescheduleProposal(Guid.NewGuid(), "Unknown", context.Today, context.Today, "Invalid")], "invalid-test-provider"));
}

public static class ApiTest
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
    public static async Task Authenticate(HttpClient client, string prefix)
    {
        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest($"{prefix}-{Guid.NewGuid():N}@example.com", "SecurePass123!", "Test Learner")); response.EnsureSuccessStatusCode();
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(Json); client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth!.Token);
    }
    public static async Task<RoadmapDto> Roadmap(HttpClient client, int duration = 4)
    {
        var response = await client.PostAsJsonAsync("/api/roadmaps", new RoadmapRequest("Backend", null, duration, DateOnly.FromDateTime(DateTime.UtcNow))); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<RoadmapDto>(Json))!;
    }
    public static async Task<WeekDto> Week(HttpClient client, Guid roadmapId, int number = 1)
    {
        var response = await client.PostAsJsonAsync($"/api/roadmaps/{roadmapId}/weeks", new WeekRequest(number, $"Week {number}"));
        if (!response.IsSuccessStatusCode) throw new HttpRequestException($"Add week for roadmap {roadmapId} failed with {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<WeekDto>(Json))!;
    }
    public static async Task<TaskDto> Task(HttpClient client, Guid weekId, DateOnly planned, DateOnly? deadline = null)
    {
        var request = new TaskRequest("Exercise", null, LearningTaskType.Exercise, LearningTaskStatus.Todo, TaskPriority.High, planned, deadline ?? planned, 45, null);
        var response = await client.PostAsJsonAsync($"/api/weeks/{weekId}/tasks", request); response.EnsureSuccessStatusCode(); return (await response.Content.ReadFromJsonAsync<TaskDto>(Json))!;
    }
}

public class AuthenticationTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    [Fact] public async Task Registration_WithValidDetails_ReturnsToken() { var client = factory.CreateClient(); var email = $"register-{Guid.NewGuid():N}@example.com"; var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "SecurePass123!", "Learner")); Assert.Equal(HttpStatusCode.OK, response.StatusCode); Assert.False(string.IsNullOrWhiteSpace((await response.Content.ReadFromJsonAsync<AuthResponse>(ApiTest.Json))!.Token)); }
    [Fact] public async Task Login_WithWrongPassword_ReturnsUnauthorized() { var client = factory.CreateClient(); var email = $"login-{Guid.NewGuid():N}@example.com"; await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, "CorrectPass123!", "Learner")); var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "WrongPass123!")); Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); }
    [Fact] public async Task RegisterPreflight_FromConfiguredFrontend_ReturnsCorsHeaders() { var client = factory.CreateClient(); var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/register"); request.Headers.Add("Origin", "http://localhost:5173"); request.Headers.Add("Access-Control-Request-Method", "POST"); request.Headers.Add("Access-Control-Request-Headers", "content-type,authorization"); var response = await client.SendAsync(request); Assert.Equal(HttpStatusCode.NoContent, response.StatusCode); Assert.Equal("http://localhost:5173", response.Headers.GetValues("Access-Control-Allow-Origin").Single()); Assert.Contains("POST", response.Headers.GetValues("Access-Control-Allow-Methods").Single()); var headers = response.Headers.GetValues("Access-Control-Allow-Headers").Single(); Assert.Contains("content-type", headers, StringComparison.OrdinalIgnoreCase); Assert.Contains("authorization", headers, StringComparison.OrdinalIgnoreCase); }
}

public class WeekAndTaskTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    [Fact] public async Task WeekNumber_CannotExceedRoadmapDuration() { var c = factory.CreateClient(); await ApiTest.Authenticate(c, "week-range"); var r = await ApiTest.Roadmap(c, 2); var response = await c.PostAsJsonAsync($"/api/roadmaps/{r.Id}/weeks", new WeekRequest(3, "Invalid")); Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); }
    [Fact] public async Task Week_CannotBeChangedByAnotherUser() { var owner = factory.CreateClient(); var other = factory.CreateClient(); await ApiTest.Authenticate(owner, "week-owner"); await ApiTest.Authenticate(other, "week-other"); var week = await ApiTest.Week(owner, (await ApiTest.Roadmap(owner)).Id); var response = await other.PutAsJsonAsync($"/api/weeks/{week.Id}", new WeekRequest(1, "Changed")); Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); }
    [Fact] public async Task Task_CannotBeReadByAnotherUser() { var owner = factory.CreateClient(); var other = factory.CreateClient(); await ApiTest.Authenticate(owner, "task-owner"); await ApiTest.Authenticate(other, "task-other"); var task = await ApiTest.Task(owner, (await ApiTest.Week(owner, (await ApiTest.Roadmap(owner)).Id)).Id, DateOnly.FromDateTime(DateTime.UtcNow)); Assert.Equal(HttpStatusCode.NotFound, (await other.GetAsync($"/api/tasks/{task.Id}")).StatusCode); }
    [Fact] public async Task CompleteThenIncomplete_MaintainsTimestamp() { var c = factory.CreateClient(); await ApiTest.Authenticate(c, "timestamp"); var task = await ApiTest.Task(c, (await ApiTest.Week(c, (await ApiTest.Roadmap(c)).Id)).Id, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)); var completed = await (await c.PatchAsJsonAsync($"/api/tasks/{task.Id}/completion", true)).Content.ReadFromJsonAsync<TaskDto>(ApiTest.Json); Assert.NotNull(completed!.CompletedAtUtc); Assert.False(completed.IsOverdue); var incomplete = await (await c.PatchAsJsonAsync($"/api/tasks/{task.Id}/completion", false)).Content.ReadFromJsonAsync<TaskDto>(ApiTest.Json); Assert.Null(incomplete!.CompletedAtUtc); Assert.True(incomplete.IsOverdue); }
}

public class ReminderAndProgressTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    [Fact] public async Task Reminder_CannotBeMarkedReadByAnotherUser() { var owner = factory.CreateClient(); var other = factory.CreateClient(); await ApiTest.Authenticate(owner, "rem-owner"); await ApiTest.Authenticate(other, "rem-other"); var week = await ApiTest.Week(owner, (await ApiTest.Roadmap(owner)).Id); await ApiTest.Task(owner, week.Id, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)); var generated = await (await owner.PostAsync("/api/reminders/generate", null)).Content.ReadFromJsonAsync<List<ReminderDto>>(ApiTest.Json); Assert.Single(generated!); Assert.StartsWith("Gecikmiş görev:", generated![0].Message); Assert.Equal(HttpStatusCode.NotFound, (await other.PatchAsync($"/api/reminders/{generated[0].Id}/read", null)).StatusCode); var duplicate = await (await owner.PostAsync("/api/reminders/generate", null)).Content.ReadFromJsonAsync<List<ReminderDto>>(ApiTest.Json); Assert.Empty(duplicate!); }
    [Fact] public async Task RoadmapProgress_IsCompletedPercentage() { var c = factory.CreateClient(); await ApiTest.Authenticate(c, "progress"); var roadmap = await ApiTest.Roadmap(c); var week = await ApiTest.Week(c, roadmap.Id); var first = await ApiTest.Task(c, week.Id, DateOnly.FromDateTime(DateTime.UtcNow)); await ApiTest.Task(c, week.Id, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)); await c.PatchAsJsonAsync($"/api/tasks/{first.Id}/completion", true); var updated = await (await c.GetAsync($"/api/roadmaps/{roadmap.Id}")).Content.ReadFromJsonAsync<RoadmapDto>(ApiTest.Json); Assert.Equal(50, updated!.Progress); Assert.Equal(50, updated.Weeks[0].Progress); }
}

public class BusinessRuleTests
{
    [Fact] public void CompletedTask_WithPastDeadline_IsNotOverdue() { var today = new DateOnly(2026, 8, 17); var task = new LearningTask { Title = "Done", Status = LearningTaskStatus.Completed, PlannedDate = today.AddDays(-2), Deadline = today.AddDays(-1) }; Assert.False(task.IsOverdue(today)); }
    [Theory]
    [InlineData(AiActionType.TodayRecommendation, "Bugünkü çalışma önerisi")]
    [InlineData(AiActionType.RescheduleUnfinished, "Yeniden planlama önerisi")]
    [InlineData(AiActionType.WeeklySummary, "Haftalık ilerleme özeti")]
    [InlineData(AiActionType.NextTask, "Sonraki öğrenme görevi")]
    [InlineData(AiActionType.FallingBehind, "Geride kalma açıklaması")]
    public async Task Fallback_ReturnsActionSpecificValidResult(AiActionType action, string expectedTitle) { var today = new DateOnly(2026, 8, 17); var task = new TaskDto(Guid.NewGuid(), "Late task", null, LearningTaskType.Exercise, LearningTaskStatus.Todo, TaskPriority.High, today.AddDays(-2), today.AddDays(-1), 30, null, null, Guid.NewGuid(), true); var result = await new FallbackAiLearningService().SuggestAsync(new AiLearningContext(["Backend"], [task], [], today), action, default); Assert.Equal(expectedTitle, result.Title); Assert.False(string.IsNullOrWhiteSpace(result.Summary)); Assert.Equal(FallbackAiLearningService.Provider, result.ProviderIdentifier); Assert.Equal(action == AiActionType.RescheduleUnfinished, result.RescheduleProposals.Count > 0); }
}

public class AiApplyTests(TestApiFactory factory) : IClassFixture<TestApiFactory>
{
    [Fact] public async Task CompletedTask_CannotBeAppliedFromRescheduleProposal() { var c = factory.CreateClient(); await ApiTest.Authenticate(c, "ai-complete"); var task = await ApiTest.Task(c, (await ApiTest.Week(c, (await ApiTest.Roadmap(c)).Id)).Id, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)); var suggestion = await (await c.PostAsync("/api/ai/reschedule", null)).Content.ReadFromJsonAsync<AiSuggestion>(ApiTest.Json); await c.PatchAsJsonAsync($"/api/tasks/{task.Id}/completion", true); var response = await c.PostAsJsonAsync($"/api/ai/recommendations/{suggestion!.RecommendationId}/apply-reschedule", new ApplyRescheduleRequest([task.Id])); Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); }
    [Fact] public async Task AnotherUser_CannotApplyRecommendation() { var owner = factory.CreateClient(); var other = factory.CreateClient(); await ApiTest.Authenticate(owner, "ai-owner"); await ApiTest.Authenticate(other, "ai-other"); var task = await ApiTest.Task(owner, (await ApiTest.Week(owner, (await ApiTest.Roadmap(owner)).Id)).Id, DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2)); var suggestion = await (await owner.PostAsync("/api/ai/reschedule", null)).Content.ReadFromJsonAsync<AiSuggestion>(ApiTest.Json); var response = await other.PostAsJsonAsync($"/api/ai/recommendations/{suggestion!.RecommendationId}/apply-reschedule", new ApplyRescheduleRequest([task.Id])); Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); }
    [Fact] public async Task InvalidTaskReference_IsRejected() { var c = factory.CreateClient(); await ApiTest.Authenticate(c, "ai-invalid"); var response = await c.PostAsJsonAsync($"/api/ai/recommendations/{Guid.NewGuid()}/apply-reschedule", new ApplyRescheduleRequest([Guid.NewGuid()])); Assert.Equal(HttpStatusCode.NotFound, response.StatusCode); }
}

public class AiProviderValidationTests(InvalidAiFactory factory) : IClassFixture<InvalidAiFactory>
{
    [Fact] public async Task InvalidProviderTaskReference_IsRejected() { var client = factory.CreateClient(); await ApiTest.Authenticate(client, "invalid-provider"); var response = await client.PostAsync("/api/ai/reschedule", null); Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode); }
}
