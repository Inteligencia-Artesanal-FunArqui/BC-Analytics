using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.CreatedUpdatedDate.Extensions;
using OsitoPolar.Analytics.Service.Infrastructure.Persistence.EFC.Configuration.Extensions;
using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;

namespace OsitoPolar.Analytics.Service.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Analytics Bounded Context database context
/// </summary>
public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : DbContext(options)
{
    // Analytics
    public DbSet<TemperatureReading> TemperatureReadings { get; set; }
    public DbSet<EnergyReading> EnergyReadings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Add the created and updated interceptor
        builder.AddCreatedUpdatedInterceptor();
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply Analytics context configuration
        builder.ApplyAnalyticsConfiguration();

        // Apply snake_case naming convention
        builder.UseSnakeCaseNamingConvention();
    }
}
