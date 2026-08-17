using System.ComponentModel.DataAnnotations;
using DevTrack.Domain;

namespace DevTrack.Application;

public record RegisterRequest([Required, EmailAddress] string Email, [Required, MinLength(8)] string Password, [Required, MaxLength(80)] string DisplayName);
public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
public record AuthResponse(string Token, string DisplayName, string Email);
public record RoadmapRequest([Required, MaxLength(150)] string Title, string? Description, [Range(1, 104)] int DurationWeeks, DateOnly StartDate);
public record WeekRequest([Range(1, 104)] int WeekNumber, [Required, MaxLength(150)] string Title);
public record TaskRequest([Required, MaxLength(180)] string Title, string? Description, LearningTaskType Type, LearningTaskStatus Status, TaskPriority Priority, DateOnly PlannedDate, DateOnly? Deadline, [Range(1, 1440)] int EstimatedMinutes, [Url] string? ResourceUrl);
public record TaskDto(Guid Id, string Title, string? Description, LearningTaskType Type, LearningTaskStatus Status, TaskPriority Priority, DateOnly PlannedDate, DateOnly? Deadline, int EstimatedMinutes, DateTime? CompletedAtUtc, string? ResourceUrl, Guid RoadmapWeekId, bool IsOverdue);
public record WeekDto(Guid Id, int WeekNumber, string Title, double Progress, IReadOnlyList<TaskDto> Tasks);
public record RoadmapDto(Guid Id, string Title, string? Description, int DurationWeeks, DateOnly StartDate, double Progress, IReadOnlyList<WeekDto> Weeks);
public record DashboardDto(IReadOnlyList<TaskDto> Today, IReadOnlyList<TaskDto> ThisWeek, IReadOnlyList<TaskDto> Overdue, IReadOnlyList<TaskDto> RecentlyCompleted, IReadOnlyList<TaskDto> UpcomingDeadlines, int Completed, int Remaining, double Progress, int Streak, IReadOnlyList<string> Reminders);
public record ReminderDto(Guid Id, string Message, ReminderType Type, bool IsRead, DateTime CreatedAtUtc, Guid? LearningTaskId);
public record AiRequest(AiActionType Action);
public record AiRescheduleProposal(Guid TaskId, string TaskTitle, DateOnly ProposedPlannedDate, DateOnly? ProposedDeadline, string Reason);
public record AiSuggestion(Guid RecommendationId, AiActionType Action, string Title, string Summary, IReadOnlyList<string> Steps, IReadOnlyList<AiRescheduleProposal> RescheduleProposals, string ProviderIdentifier, string Disclaimer = "AI önerileri incelemeni gerektirir; hiçbir veri otomatik olarak değiştirilmedi.");
public record AiProviderResult(string Title, string Summary, IReadOnlyList<string> Steps, IReadOnlyList<AiRescheduleProposal> RescheduleProposals, string ProviderIdentifier);
public record AiLearningContext(IReadOnlyList<string> Roadmaps, IReadOnlyList<TaskDto> Incomplete, IReadOnlyList<TaskDto> Completed, DateOnly Today);
public record ApplyRescheduleRequest([Required, MinLength(1)] IReadOnlyList<Guid> TaskIds);
public record ApplyRescheduleResponse(Guid RecommendationId, int AppliedTaskCount);

public interface IAiLearningService { Task<AiProviderResult> SuggestAsync(AiLearningContext context, AiActionType action, CancellationToken cancellationToken); }

public static class Mapping
{
    public static TaskDto ToDto(this LearningTask task, DateOnly today) => new(task.Id, task.Title, task.Description, task.Type, task.Status, task.Priority, task.PlannedDate, task.Deadline, task.EstimatedMinutes, task.CompletedAtUtc, task.ResourceUrl, task.RoadmapWeekId, task.IsOverdue(today));
    public static double Percent(int complete, int total) => total == 0 ? 0 : Math.Round(complete * 100d / total, 1);
}
