using DevTrack.Application;
using DevTrack.Domain;

namespace DevTrack.Infrastructure;

public class FallbackAiLearningService : IAiLearningService
{
    public const string Provider = "local-fallback-v1";

    public Task<AiProviderResult> SuggestAsync(AiLearningContext context, AiActionType action, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(action switch
        {
            AiActionType.TodayRecommendation => Today(context),
            AiActionType.RescheduleUnfinished => Reschedule(context),
            AiActionType.WeeklySummary => WeeklySummary(context),
            AiActionType.NextTask => NextTask(context),
            AiActionType.FallingBehind => FallingBehind(context),
            _ => throw new ArgumentOutOfRangeException(nameof(action), "Unsupported AI action.")
        });
    }

    private static AiProviderResult Today(AiLearningContext context)
    {
        var tasks = context.Incomplete.Where(x => x.PlannedDate <= context.Today || x.Deadline == context.Today)
            .OrderByDescending(x => x.IsOverdue).ThenBy(x => x.Deadline ?? x.PlannedDate).ThenByDescending(x => x.Priority).Take(3).ToList();
        return tasks.Count == 0
            ? Result("Bugünkü çalışma önerisi", "Bugün teslim edilmesi gereken bir görevin yok.", ["Son çalışmalarını gözden geçir veya roadmapindeki sıradaki görevi planla."])
            : Result("Bugünkü çalışma önerisi", $"{tasks.Count} göreve odaklan. Toplam tahmini süre yaklaşık {tasks.Sum(x => x.EstimatedMinutes)} dakika.", tasks.Select(x => $"{x.Title} — {x.EstimatedMinutes} dakika").ToList());
    }

    private static AiProviderResult Reschedule(AiLearningContext context)
    {
        var tasks = context.Incomplete.Where(x => x.PlannedDate < context.Today || x.IsOverdue).OrderBy(x => x.Deadline ?? x.PlannedDate).Take(5).ToList();
        var proposals = tasks.Select((task, index) =>
        {
            var planned = context.Today.AddDays(index);
            DateOnly? deadline = task.Deadline.HasValue ? planned.AddDays(Math.Max(0, task.Deadline.Value.DayNumber - task.PlannedDate.DayNumber)) : null;
            return new AiRescheduleProposal(task.Id, task.Title, planned, deadline, "Tamamlanmayan çalışmayı uygun olan sonraki çalışma zamanına taşı.");
        }).ToList();
        return new("Yeniden planlama önerisi", proposals.Count == 0 ? "Yeniden planlanması gereken gecikmiş bir görevin yok." : $"{proposals.Count} tamamlanmamış görev için yeni tarihler önerildi.", proposals.Select(x => $"{x.TaskTitle}: {x.ProposedPlannedDate:yyyy-MM-dd}").ToList(), proposals, Provider);
    }

    private static AiProviderResult WeeklySummary(AiLearningContext context)
    {
        var start = context.Today.AddDays(-((int)context.Today.DayOfWeek + 6) % 7); var end = start.AddDays(6);
        var all = context.Incomplete.Concat(context.Completed).Where(x => x.PlannedDate >= start && x.PlannedDate <= end).ToList();
        var complete = all.Count(x => x.Status == LearningTaskStatus.Completed);
        return Result("Haftalık ilerleme özeti", $"Bu hafta planlanan {all.Count} görevin {complete} tanesini tamamladın (%{Mapping.Percent(complete, all.Count)}).", all.Count == 0 ? ["Bu hafta için planlanmış görev yok."] : [$"{all.Count - complete} görev kaldı."]);
    }

    private static AiProviderResult NextTask(AiLearningContext context)
    {
        var task = context.Incomplete.OrderByDescending(x => x.IsOverdue).ThenBy(x => x.Deadline ?? x.PlannedDate).ThenByDescending(x => x.Priority).FirstOrDefault();
        return task is null ? Result("Sonraki öğrenme görevi", "Tamamlanmamış görevin yok.", ["Roadmapine sıradaki görevi ekleyebilirsin."])
            : Result("Sonraki öğrenme görevi", $"“{task.Title}” göreviyle devam et.", [$"Bu görev için yaklaşık {task.EstimatedMinutes} dakika ayır.", task.IsOverdue ? "Bu görev gecikmiş durumda; öncelik vermen iyi olur." : "Planlanan görevler içinde tarihi ve önceliği en yakın olan görev bu."]);
    }

    private static AiProviderResult FallingBehind(AiLearningContext context)
    {
        var overdue = context.Incomplete.Where(x => x.IsOverdue).ToList(); var past = context.Incomplete.Count(x => x.PlannedDate < context.Today);
        var summary = overdue.Count == 0 && past == 0 ? "Görev tarihlerine göre şu anda geride kaldığını gösteren bir durum yok." : $"{overdue.Count} gecikmiş görevin ve bugünden önce planlanmış {past} tamamlanmamış görevin var.";
        return Result("Geride kalma açıklaması", summary, overdue.OrderBy(x => x.Deadline).Take(3).Select(x => $"“{x.Title}” görevinin kapsamını ve zamanlamasını gözden geçir.").DefaultIfEmpty("Mevcut planı gerçekçi tut ve her hafta gözden geçir.").ToList());
    }

    private static AiProviderResult Result(string title, string summary, IReadOnlyList<string> steps) => new(title, summary, steps, [], Provider);
}
