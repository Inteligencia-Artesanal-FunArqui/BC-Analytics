using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Shared.Domain.Repositories;

namespace OsitoPolar.Analytics.Service.Domain.Repositories;

/// <summary>
/// Repository interface for analytics data operations
/// </summary>
public interface IAnalyticsRepository : IBaseRepository<TemperatureReading>
{
    // Temperature Reading operations
    Task<IEnumerable<TemperatureReading>> FindTemperatureReadingsByEquipmentIdAsync(int equipmentId, int hours = 24);
    Task<IEnumerable<TemperatureReading>> FindTemperatureReadingsByDateRangeAsync(int equipmentId, DateTimeOffset start, DateTimeOffset end);
    
    // Daily Average operations
    Task<IEnumerable<DailyTemperatureAverage>> FindDailyAveragesByEquipmentIdAsync(int equipmentId, int days = 7);
    Task<DailyTemperatureAverage?> FindDailyAverageByEquipmentAndDateAsync(int equipmentId, DateOnly date);
    Task AddDailyAverageAsync(DailyTemperatureAverage dailyAverage);
    
    // Energy Reading operations
    Task<IEnumerable<EnergyReading>> FindEnergyReadingsByEquipmentIdAsync(int equipmentId, int hours = 24);
    Task AddEnergyReadingAsync(EnergyReading energyReading);
    Task<decimal> GetAverageEnergyConsumptionAsync(int equipmentId, int days = 30);
}