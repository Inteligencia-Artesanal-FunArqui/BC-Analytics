using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Interfaces.REST.Resources;

namespace OsitoPolar.Analytics.Service.Interfaces.REST.Transform;

/// <summary>
/// Assembles a TemperatureReadingResource from a TemperatureReading entity
/// </summary>
public static class TemperatureReadingResourceFromEntityAssembler
{
    public static TemperatureReadingResource ToResourceFromEntity(TemperatureReading entity)
    {
        return new TemperatureReadingResource(
            entity.Id,
            entity.EquipmentId,
            entity.Temperature,
            entity.Timestamp,
            entity.Status.ToString()
        );
    }
}