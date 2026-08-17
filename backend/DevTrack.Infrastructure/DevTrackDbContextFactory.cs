using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DevTrack.Infrastructure;

/// <summary>
/// Creates the database context for EF Core CLI commands without starting the API.
/// Runtime dependency injection remains configured in DevTrack.Api/Program.cs.
/// </summary>
public class DevTrackDbContextFactory : IDesignTimeDbContextFactory<DevTrackDbContext>
{
    public DevTrackDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DevTrack")
            ?? throw new InvalidOperationException(
                "Set ConnectionStrings__DevTrack before running EF Core commands.");

        var options = new DbContextOptionsBuilder<DevTrackDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new DevTrackDbContext(options);
    }
}
