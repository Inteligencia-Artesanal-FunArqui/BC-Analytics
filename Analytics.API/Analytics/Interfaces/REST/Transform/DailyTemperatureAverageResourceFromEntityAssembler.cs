using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Interfaces.REST.Resources;

namespace OsitoPolar.Analytics.Service.Interfaces.REST.Transform;

/// <summary>
/// Assembles a DailyTemperatureAverageResource from a DailyTemperatureAverage entity
/// </summary>
public static class DailyTemperatureAverageResourceFromEntityAssembler
{
    public static DailyTemperatureAverageResource ToResourceFromEntity(DailyTemperatureAverage entity)
    {
        return new DailyTemperatureAverageResource(
            entity.Id,
            entity.EquipmentId,
            entity.Date.ToString("yyyy-MM-dd"),
            entity.AverageTemperature,
            entity.MinTemperature,
            entity.MaxTemperature
        );
    }
}