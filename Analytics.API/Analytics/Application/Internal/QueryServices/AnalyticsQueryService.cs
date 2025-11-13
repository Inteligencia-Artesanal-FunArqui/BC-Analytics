using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Domain.Model.Queries;
using OsitoPolar.Analytics.Service.Domain.Repositories;
using OsitoPolar.Analytics.Service.Domain.Services;

namespace OsitoPolar.Analytics.Service.Application.Internal.QueryServices;

/// <summary>
/// Implementation of analytics query service
/// </summary>
public class AnalyticsQueryService(IAnalyticsRepository analyticsRepository) : IAnalyticsQueryService
{
    public async Task<IEnumerable<TemperatureReading>> Handle(GetTemperatureReadingsQuery query)
    {
        return await analyticsRepository.FindTemperatureReadingsByEquipmentIdAsync(query.EquipmentId, query.Hours);
    }

    public async Task<IEnumerable<DailyTemperatureAverage>> Handle(GetDailyTemperatureAveragesQuery query)
    {
        return await analyticsRepository.FindDailyAveragesByEquipmentIdAsync(query.EquipmentId, query.Days);
    }

    public async Task<IEnumerable<EnergyReading>> Handle(GetEnergyReadingsQuery query)
    {
        return await analyticsRepository.FindEnergyReadingsByEquipmentIdAsync(query.EquipmentId, query.Hours);
    }
}