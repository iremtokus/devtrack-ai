using DevTrack.Domain;
using Microsoft.EntityFrameworkCore;

namespace DevTrack.Infrastructure;

public class DevTrackDbContext(DbContextOptions<DevTrackDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Roadmap> Roadmaps => Set<Roadmap>();
    public DbSet<RoadmapWeek> RoadmapWeeks => Set<RoadmapWeek>();
    public DbSet<LearningTask> LearningTasks => Set<LearningTask>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<AiRecommendation> AiRecommendations => Set<AiRecommendation>();
    public DbSet<AiRescheduleItem> AiRescheduleItems => Set<AiRescheduleItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<User>().Property(x => x.Email).HasMaxLength(254);
        b.Entity<Roadmap>().HasOne(x => x.User).WithMany(x => x.Roadmaps).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<RoadmapWeek>().HasOne(x => x.Roadmap).WithMany(x => x.Weeks).HasForeignKey(x => x.RoadmapId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<LearningTask>().HasOne(x => x.RoadmapWeek).WithMany(x => x.Tasks).HasForeignKey(x => x.RoadmapWeekId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<LearningTask>().HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Reminder>().HasOne(x => x.User).WithMany(x => x.Reminders).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Reminder>().HasOne(x => x.LearningTask).WithMany().HasForeignKey(x => x.LearningTaskId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<Reminder>().HasIndex(x => new { x.UserId, x.EventKey }).IsUnique();
        b.Entity<Reminder>().Property(x => x.Type).HasConversion<string>();
        b.Entity<AiRecommendation>().HasOne(x => x.User).WithMany(x => x.AiRecommendations).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<AiRecommendation>().HasMany(x => x.RescheduleItems).WithOne(x => x.AiRecommendation).HasForeignKey(x => x.AiRecommendationId).OnDelete(DeleteBehavior.Cascade);
        b.Entity<AiRecommendation>().Property(x => x.Action).HasConversion<string>();
        b.Entity<AiRecommendation>().Property(x => x.Status).HasConversion<string>();
        b.Entity<RoadmapWeek>().HasIndex(x => new { x.RoadmapId, x.WeekNumber }).IsUnique();
        b.Entity<LearningTask>().Property(x => x.Type).HasConversion<string>();
        b.Entity<LearningTask>().Property(x => x.Status).HasConversion<string>();
        b.Entity<LearningTask>().Property(x => x.Priority).HasConversion<string>();
    }
}
