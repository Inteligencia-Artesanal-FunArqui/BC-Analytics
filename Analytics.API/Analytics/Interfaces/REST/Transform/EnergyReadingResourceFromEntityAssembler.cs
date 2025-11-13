using OsitoPolar.Analytics.Service.Domain.Model.Entities;
using OsitoPolar.Analytics.Service.Interfaces.REST.Resources;

namespace OsitoPolar.Analytics.Service.Interfaces.REST.Transform;

/// <summary>
/// Assembles an EnergyReadingResource from an EnergyReading entity
/// </summary>
public static class EnergyReadingResourceFromEntityAssembler
{
    public static EnergyReadingResource ToResourceFromEntity(EnergyReading entity)
    {
        return new EnergyReadingResource(
            entity.Id,
            entity.EquipmentId,
            entity.Consumption,
            entity.Unit,
            entity.Timestamp,
            entity.Status.ToString()
        );
    }
}