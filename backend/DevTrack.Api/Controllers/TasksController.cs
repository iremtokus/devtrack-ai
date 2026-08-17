using DevTrack.Application;
using DevTrack.Domain;
using DevTrack.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Api.Controllers;

[Authorize]
public class TasksController(DevTrackDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> All([FromQuery] LearningTaskStatus? status, [FromQuery] Guid? roadmapId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
    {
        if (from > to) return BadRequest(new ProblemDetails { Title = "Başlangıç tarihi bitiş tarihinden sonra olamaz." });
        var query = db.LearningTasks.AsNoTracking().Where(x => x.UserId == UserId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        if (roadmapId.HasValue) query = query.Where(x => x.RoadmapWeek!.RoadmapId == roadmapId);
        if (from.HasValue) query = query.Where(x => x.PlannedDate >= from);
        if (to.HasValue) query = query.Where(x => x.PlannedDate <= to);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Ok((await query.OrderBy(x => x.PlannedDate).ThenByDescending(x => x.Priority).ToListAsync()).Select(x => x.ToDto(today)).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Get(Guid id)
    {
        var task = await db.LearningTasks.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        return task is null ? NotFound() : Ok(task.ToDto(DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [HttpGet("today")] public Task<List<TaskDto>> Today() => Range(DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow));
    [HttpGet("week")] public Task<List<TaskDto>> Week() { var day = DateOnly.FromDateTime(DateTime.UtcNow); var start = day.AddDays(-((int)day.DayOfWeek + 6) % 7); return Range(start, start.AddDays(6)); }

    [HttpPost("/api/weeks/{weekId:guid}/tasks")]
    public async Task<ActionResult<TaskDto>> Create(Guid weekId, TaskRequest request)
    {
        if (!Validate(request, out var error)) return BadRequest(new ProblemDetails { Title = error });
        if (!await db.RoadmapWeeks.AnyAsync(x => x.Id == weekId && x.Roadmap!.UserId == UserId)) return NotFound();
        var task = Apply(new LearningTask { Title = request.Title, RoadmapWeekId = weekId, UserId = UserId }, request);
        db.Add(task); await db.SaveChangesAsync(); return Ok(task.ToDto(DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskDto>> Update(Guid id, TaskRequest request)
    {
        if (!Validate(request, out var error)) return BadRequest(new ProblemDetails { Title = error });
        var task = await db.LearningTasks.SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (task is null) return NotFound();
        Apply(task, request); await db.SaveChangesAsync(); return Ok(task.ToDto(DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [HttpPatch("{id:guid}/completion")]
    public async Task<ActionResult<TaskDto>> Complete(Guid id, [FromBody] bool completed)
    {
        var task = await db.LearningTasks.SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (task is null) return NotFound();
        if (completed && task.Status != LearningTaskStatus.Completed) { task.Status = LearningTaskStatus.Completed; task.CompletedAtUtc = DateTime.UtcNow; }
        else if (!completed) { task.Status = LearningTaskStatus.Todo; task.CompletedAtUtc = null; }
        await db.SaveChangesAsync(); return Ok(task.ToDto(DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await db.LearningTasks.SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (task is null) return NotFound();
        db.Remove(task); await db.SaveChangesAsync(); return NoContent();
    }

    private async Task<List<TaskDto>> Range(DateOnly from, DateOnly to)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return (await db.LearningTasks.AsNoTracking().Where(x => x.UserId == UserId && x.PlannedDate >= from && x.PlannedDate <= to).OrderBy(x => x.PlannedDate).ToListAsync()).Select(x => x.ToDto(today)).ToList();
    }

    private static bool Validate(TaskRequest request, out string error)
    {
        if (request.PlannedDate == default) { error = "Planlanan tarih zorunludur."; return false; }
        if (request.Deadline.HasValue && request.Deadline.Value < request.PlannedDate) { error = "Son tarih planlanan tarihten önce olamaz."; return false; }
        if (!string.IsNullOrWhiteSpace(request.ResourceUrl) && (!Uri.TryCreate(request.ResourceUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))) { error = "Kaynak URL geçerli bir HTTP veya HTTPS adresi olmalıdır."; return false; }
        error = string.Empty; return true;
    }

    private static LearningTask Apply(LearningTask task, TaskRequest request)
    {
        var wasCompleted = task.Status == LearningTaskStatus.Completed;
        task.Title = request.Title.Trim(); task.Description = request.Description; task.Type = request.Type; task.Priority = request.Priority; task.PlannedDate = request.PlannedDate; task.Deadline = request.Deadline; task.EstimatedMinutes = request.EstimatedMinutes; task.ResourceUrl = request.ResourceUrl; task.Status = request.Status;
        task.CompletedAtUtc = request.Status == LearningTaskStatus.Completed ? (wasCompleted ? task.CompletedAtUtc ?? DateTime.UtcNow : DateTime.UtcNow) : null;
        return task;
    }
}
