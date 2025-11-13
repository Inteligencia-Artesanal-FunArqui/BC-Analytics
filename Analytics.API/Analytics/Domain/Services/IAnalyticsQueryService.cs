using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Domain.Model.Queries;

namespace OsitoPolar.Analytics.Service.Domain.Services;

/// <summary>
/// Query service interface for analytics operations
/// </summary>
public interface IAnalyticsQueryService
{
    Task<IEnumerable<TemperatureReading>> Handle(GetTemperatureReadingsQuery query);
    Task<IEnumerable<DailyTemperatureAverage>> Handle(GetDailyTemperatureAveragesQuery query);
    Task<IEnumerable<EnergyReading>> Handle(GetEnergyReadingsQuery query);
}