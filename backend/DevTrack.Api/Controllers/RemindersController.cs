using DevTrack.Application;
using DevTrack.Domain;
using DevTrack.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Api.Controllers;

[Authorize]
public class RemindersController(DevTrackDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ReminderDto>> All([FromQuery] bool unreadOnly = false)
    {
        var query = db.Reminders.AsNoTracking().Where(x => x.UserId == UserId);
        if (unreadOnly) query = query.Where(x => !x.IsRead);
        return (await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync()).Select(Map).ToList();
    }

    [HttpPost("generate")]
    public async Task<IReadOnlyList<ReminderDto>> Generate()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var tasks = await db.LearningTasks.AsNoTracking().Where(x => x.UserId == UserId && x.Status != LearningTaskStatus.Completed && (x.PlannedDate == today || x.Deadline == today || x.Deadline < today)).ToListAsync();
        var existingKeys = (await db.Reminders.Where(x => x.UserId == UserId).Select(x => x.EventKey).ToListAsync()).ToHashSet();
        var created = new List<Reminder>();
        foreach (var task in tasks)
        {
            var type = task.Deadline < today ? ReminderType.Overdue : ReminderType.DueToday;
            var key = $"{type}:{task.Id}";
            if (existingKeys.Contains(key)) continue;
            var reminder = new Reminder { UserId = UserId, LearningTaskId = task.Id, Type = type, EventKey = key, Message = type == ReminderType.Overdue ? $"Gecikmiş görev: {task.Title}" : $"Bugün tamamlanacak: {task.Title}" };
            db.Reminders.Add(reminder); created.Add(reminder); existingKeys.Add(key);
        }
        await db.SaveChangesAsync(); return created.Select(Map).ToList();
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<ActionResult<ReminderDto>> MarkRead(Guid id)
    {
        var reminder = await db.Reminders.SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (reminder is null) return NotFound();
        reminder.IsRead = true; await db.SaveChangesAsync(); return Ok(Map(reminder));
    }

    private static ReminderDto Map(Reminder x) => new(x.Id, x.Message, x.Type, x.IsRead, x.CreatedAtUtc, x.LearningTaskId);
}
