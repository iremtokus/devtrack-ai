using DevTrack.Application;
using DevTrack.Domain;
using DevTrack.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Api.Controllers;

[Authorize]
public class AiController(DevTrackDbContext db, IAiLearningService ai, ILogger<AiController> logger) : ApiControllerBase
{
    [HttpPost("today")] public Task<ActionResult<AiSuggestion>> Today(CancellationToken ct) => Generate(AiActionType.TodayRecommendation, ct);
    [HttpPost("reschedule")] public Task<ActionResult<AiSuggestion>> Reschedule(CancellationToken ct) => Generate(AiActionType.RescheduleUnfinished, ct);
    [HttpPost("weekly-summary")] public Task<ActionResult<AiSuggestion>> WeeklySummary(CancellationToken ct) => Generate(AiActionType.WeeklySummary, ct);
    [HttpPost("next-task")] public Task<ActionResult<AiSuggestion>> NextTask(CancellationToken ct) => Generate(AiActionType.NextTask, ct);
    [HttpPost("falling-behind")] public Task<ActionResult<AiSuggestion>> FallingBehind(CancellationToken ct) => Generate(AiActionType.FallingBehind, ct);
    [HttpPost("suggest")] public Task<ActionResult<AiSuggestion>> Suggest(AiRequest request, CancellationToken ct) => Generate(request.Action, ct);

    [HttpPost("recommendations/{recommendationId:guid}/apply-reschedule")]
    public async Task<ActionResult<ApplyRescheduleResponse>> ApplyReschedule(Guid recommendationId, ApplyRescheduleRequest request, CancellationToken ct)
    {
        var recommendation = await db.AiRecommendations.Include(x => x.RescheduleItems).SingleOrDefaultAsync(x => x.Id == recommendationId && x.UserId == UserId, ct);
        if (recommendation is null) return NotFound();
        if (recommendation.Action != AiActionType.RescheduleUnfinished) return BadRequest(new ProblemDetails { Title = "Bu öneri bir yeniden planlama önerisi değil." });
        if (recommendation.Status != AiRecommendationStatus.Proposed) return Conflict(new ProblemDetails { Title = "Bu öneri artık uygulanabilir durumda değil." });
        var selectedIds = request.TaskIds.Distinct().ToHashSet();
        var selected = recommendation.RescheduleItems.Where(x => selectedIds.Contains(x.LearningTaskId)).ToList();
        if (selected.Count != selectedIds.Count) return BadRequest(new ProblemDetails { Title = "Bir veya daha fazla görev bu öneriye ait değil." });
        var tasks = await db.LearningTasks.Where(x => selectedIds.Contains(x.Id) && x.UserId == UserId).ToDictionaryAsync(x => x.Id, ct);
        if (tasks.Count != selectedIds.Count) return BadRequest(new ProblemDetails { Title = "Bir veya daha fazla görev referansı geçersiz." });
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        foreach (var item in selected)
        {
            var task = tasks[item.LearningTaskId];
            if (task.Status == LearningTaskStatus.Completed) return BadRequest(new ProblemDetails { Title = $"Tamamlanmış “{task.Title}” görevi yeniden planlanamaz." });
            if (item.ProposedPlannedDate < today || item.ProposedDeadline < item.ProposedPlannedDate) return BadRequest(new ProblemDetails { Title = "Önerilen tarihler geçersiz." });
        }
        foreach (var item in selected) { var task = tasks[item.LearningTaskId]; task.PlannedDate = item.ProposedPlannedDate; task.Deadline = item.ProposedDeadline; }
        recommendation.Status = AiRecommendationStatus.Applied; await db.SaveChangesAsync(ct);
        return Ok(new ApplyRescheduleResponse(recommendation.Id, selected.Count));
    }

    private async Task<ActionResult<AiSuggestion>> Generate(AiActionType action, CancellationToken ct)
    {
        var tasks = await db.LearningTasks.AsNoTracking().Where(x => x.UserId == UserId).ToListAsync(ct);
        var roadmaps = await db.Roadmaps.AsNoTracking().Where(x => x.UserId == UserId).Select(x => x.Title).ToListAsync(ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var context = new AiLearningContext(roadmaps, tasks.Where(x => x.Status != LearningTaskStatus.Completed).Select(x => x.ToDto(today)).ToList(), tasks.Where(x => x.Status == LearningTaskStatus.Completed).Select(x => x.ToDto(today)).ToList(), today);
        try
        {
            var result = await ai.SuggestAsync(context, action, ct);
            if (string.IsNullOrWhiteSpace(result.Summary) || string.IsNullOrWhiteSpace(result.ProviderIdentifier) || result.Steps is null || result.RescheduleProposals is null)
                return StatusCode(502, new ProblemDetails { Title = "AI sağlayıcısı geçersiz bir yanıt döndürdü." });
            var incompleteIds = context.Incomplete.Select(x => x.Id).ToHashSet();
            if (result.RescheduleProposals.Any(x => !incompleteIds.Contains(x.TaskId) || x.ProposedPlannedDate < today || x.ProposedDeadline < x.ProposedPlannedDate))
                return StatusCode(502, new ProblemDetails { Title = "AI sağlayıcısı geçersiz görev referansları veya tarihler döndürdü." });
            if (action != AiActionType.RescheduleUnfinished && result.RescheduleProposals.Count > 0)
                return StatusCode(502, new ProblemDetails { Title = "AI sağlayıcısı yanlış işlem için öneri döndürdü." });
            var record = new AiRecommendation { UserId = UserId, Action = action, Summary = result.Summary, ProviderIdentifier = result.ProviderIdentifier,
                RescheduleItems = result.RescheduleProposals.Select(x => new AiRescheduleItem { LearningTaskId = x.TaskId, ProposedPlannedDate = x.ProposedPlannedDate, ProposedDeadline = x.ProposedDeadline, Reason = x.Reason }).ToList() };
            db.Add(record); await db.SaveChangesAsync(ct);
            return Ok(new AiSuggestion(record.Id, action, result.Title, result.Summary, result.Steps, result.RescheduleProposals, result.ProviderIdentifier));
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested) { return StatusCode(504, new ProblemDetails { Title = "AI sağlayıcısı zaman aşımına uğradı." }); }
        catch (Exception ex) { logger.LogWarning(ex, "AI suggestion failed without logging prompt content"); return StatusCode(503, new ProblemDetails { Title = "AI asistanı geçici olarak kullanılamıyor." }); }
    }
}
