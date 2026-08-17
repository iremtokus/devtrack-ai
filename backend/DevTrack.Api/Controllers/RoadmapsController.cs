using DevTrack.Application;
using DevTrack.Domain;
using DevTrack.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Api.Controllers;

[Authorize]
public class RoadmapsController(DevTrackDbContext db) : ApiControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<RoadmapDto>> All() =>
        (await Query().OrderBy(x => x.StartDate).ToListAsync()).Select(Map).ToList();

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoadmapDto>> Get(Guid id)
    {
        var roadmap = await Query().SingleOrDefaultAsync(x => x.Id == id);
        return roadmap is null ? NotFound() : Ok(Map(roadmap));
    }

    [HttpPost]
    public async Task<ActionResult<RoadmapDto>> Create(RoadmapRequest request)
    {
        var roadmap = new Roadmap { Title = request.Title.Trim(), Description = request.Description, DurationWeeks = request.DurationWeeks, StartDate = request.StartDate, UserId = UserId };
        db.Add(roadmap); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = roadmap.Id }, Map(roadmap));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoadmapDto>> Update(Guid id, RoadmapRequest request)
    {
        var roadmap = await db.Roadmaps.Include(x => x.Weeks).SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (roadmap is null) return NotFound();
        if (roadmap.Weeks.Any(x => x.WeekNumber > request.DurationWeeks))
            return BadRequest(new ProblemDetails { Title = "Roadmap süresi mevcut hafta numarasından kısa olamaz." });
        roadmap.Title = request.Title.Trim(); roadmap.Description = request.Description; roadmap.DurationWeeks = request.DurationWeeks; roadmap.StartDate = request.StartDate;
        await db.SaveChangesAsync(); return Ok(Map(roadmap));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var roadmap = await db.Roadmaps.SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (roadmap is null) return NotFound();
        db.Remove(roadmap); await db.SaveChangesAsync(); return NoContent();
    }

    [HttpPost("{id:guid}/weeks")]
    public async Task<ActionResult<WeekDto>> AddWeek(Guid id, WeekRequest request)
    {
        var roadmap = await db.Roadmaps.SingleOrDefaultAsync(x => x.Id == id && x.UserId == UserId);
        if (roadmap is null) return NotFound();
        if (request.WeekNumber > roadmap.DurationWeeks) return BadRequest(new ProblemDetails { Title = "Hafta numarası roadmap süresini aşamaz." });
        if (await db.RoadmapWeeks.AnyAsync(x => x.RoadmapId == id && x.WeekNumber == request.WeekNumber)) return Conflict(new ProblemDetails { Title = "Bu hafta numarası zaten mevcut." });
        var week = new RoadmapWeek { RoadmapId = id, WeekNumber = request.WeekNumber, Title = request.Title.Trim() };
        db.Add(week); await db.SaveChangesAsync(); return Ok(new WeekDto(week.Id, week.WeekNumber, week.Title, 0, []));
    }

    [HttpPut("/api/weeks/{weekId:guid}")]
    public async Task<ActionResult<WeekDto>> UpdateWeek(Guid weekId, WeekRequest request)
    {
        var week = await db.RoadmapWeeks.Include(x => x.Roadmap).Include(x => x.Tasks).SingleOrDefaultAsync(x => x.Id == weekId && x.Roadmap!.UserId == UserId);
        if (week is null) return NotFound();
        if (request.WeekNumber > week.Roadmap!.DurationWeeks) return BadRequest(new ProblemDetails { Title = "Hafta numarası roadmap süresini aşamaz." });
        if (await db.RoadmapWeeks.AnyAsync(x => x.RoadmapId == week.RoadmapId && x.WeekNumber == request.WeekNumber && x.Id != weekId)) return Conflict(new ProblemDetails { Title = "Bu hafta numarası zaten mevcut." });
        week.WeekNumber = request.WeekNumber; week.Title = request.Title.Trim(); await db.SaveChangesAsync();
        return Ok(MapWeek(week, DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [HttpDelete("/api/weeks/{weekId:guid}")]
    public async Task<IActionResult> DeleteWeek(Guid weekId)
    {
        var week = await db.RoadmapWeeks.SingleOrDefaultAsync(x => x.Id == weekId && x.Roadmap!.UserId == UserId);
        if (week is null) return NotFound();
        db.Remove(week); await db.SaveChangesAsync(); return NoContent();
    }

    private IQueryable<Roadmap> Query() => db.Roadmaps.AsNoTracking().Where(x => x.UserId == UserId).Include(x => x.Weeks).ThenInclude(x => x.Tasks);
    private static RoadmapDto Map(Roadmap x)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow); var all = x.Weeks.SelectMany(w => w.Tasks).ToList();
        return new(x.Id, x.Title, x.Description, x.DurationWeeks, x.StartDate, Mapping.Percent(all.Count(t => t.Status == LearningTaskStatus.Completed), all.Count), x.Weeks.OrderBy(w => w.WeekNumber).Select(w => MapWeek(w, today)).ToList());
    }
    private static WeekDto MapWeek(RoadmapWeek week, DateOnly today) => new(week.Id, week.WeekNumber, week.Title, Mapping.Percent(week.Tasks.Count(x => x.Status == LearningTaskStatus.Completed), week.Tasks.Count), week.Tasks.OrderBy(x => x.PlannedDate).Select(x => x.ToDto(today)).ToList());
}
