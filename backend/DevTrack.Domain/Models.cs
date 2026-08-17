namespace DevTrack.Domain;

public enum LearningTaskType { Course, Video, Reading, Exercise, Project, Review }
public enum LearningTaskStatus { Todo, InProgress, Completed }
public enum TaskPriority { Low, Medium, High }
public enum ReminderType { DueToday, Overdue }
public enum AiActionType { TodayRecommendation, RescheduleUnfinished, WeeklySummary, NextTask, FallingBehind }
public enum AiRecommendationStatus { Proposed, Applied, Rejected, Failed }

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string DisplayName { get; set; } = "Learner";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<Roadmap> Roadmaps { get; set; } = [];
    public List<Reminder> Reminders { get; set; } = [];
    public List<AiRecommendation> AiRecommendations { get; set; } = [];
}

public class Roadmap
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int DurationWeeks { get; set; }
    public DateOnly StartDate { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public List<RoadmapWeek> Weeks { get; set; } = [];
}

public class RoadmapWeek
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int WeekNumber { get; set; }
    public required string Title { get; set; }
    public Guid RoadmapId { get; set; }
    public Roadmap? Roadmap { get; set; }
    public List<LearningTask> Tasks { get; set; } = [];
}

public class LearningTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public LearningTaskType Type { get; set; }
    public LearningTaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateOnly PlannedDate { get; set; }
    public DateOnly? Deadline { get; set; }
    public int EstimatedMinutes { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ResourceUrl { get; set; }
    public Guid RoadmapWeekId { get; set; }
    public RoadmapWeek? RoadmapWeek { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public bool IsOverdue(DateOnly today) => Status != LearningTaskStatus.Completed && Deadline < today;
}

public class Reminder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid? LearningTaskId { get; set; }
    public LearningTask? LearningTask { get; set; }
    public ReminderType Type { get; set; }
    public required string EventKey { get; set; }
}

public class AiRecommendation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public AiActionType Action { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public AiRecommendationStatus Status { get; set; } = AiRecommendationStatus.Proposed;
    public required string Summary { get; set; }
    public required string ProviderIdentifier { get; set; }
    public List<AiRescheduleItem> RescheduleItems { get; set; } = [];
}

public class AiRescheduleItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AiRecommendationId { get; set; }
    public AiRecommendation? AiRecommendation { get; set; }
    public Guid LearningTaskId { get; set; }
    public DateOnly ProposedPlannedDate { get; set; }
    public DateOnly? ProposedDeadline { get; set; }
    public required string Reason { get; set; }
}
