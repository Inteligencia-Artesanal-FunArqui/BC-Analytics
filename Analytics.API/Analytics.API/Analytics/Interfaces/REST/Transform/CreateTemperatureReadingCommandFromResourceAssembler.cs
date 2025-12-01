using OsitoPolar.Analytics.Service.Domain.Model.Commands;
using OsitoPolar.Analytics.Service.Interfaces.REST.Resources;

namespace OsitoPolar.Analytics.Service.Interfaces.REST.Transform;

/// <summary>
/// Assembles a RecordTemperatureReadingCommand from a CreateTemperatureReadingResource
/// </summary>
public static class CreateTemperatureReadingCommandFromResourceAssembler
{
    public static RecordTemperatureReadingCommand ToCommandFromResource(CreateTemperatureReadingResource resource)
    {
        return new RecordTemperatureReadingCommand(
            resource.EquipmentId,
            resource.Temperature,
            resource.Timestamp
        );
    }
}