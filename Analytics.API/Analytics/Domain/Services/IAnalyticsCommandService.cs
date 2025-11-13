using OsitoPolar.Analytics.Service.Domain.Model.Commands;
using OsitoPolar.Analytics.Service.Domain.Model.Entities;

namespace OsitoPolar.Analytics.Service.Domain.Services;

/// <summary>
/// Command service interface for analytics operations
/// </summary>
public interface IAnalyticsCommandService
{
    Task<TemperatureReading?> Handle(RecordTemperatureReadingCommand command);
    Task<EnergyReading?> Handle(RecordEnergyReadingCommand command);
}