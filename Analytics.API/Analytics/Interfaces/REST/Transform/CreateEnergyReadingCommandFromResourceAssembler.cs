using OsitoPolar.Analytics.Service.Domain.Model.Commands;
using OsitoPolar.Analytics.Service.Interfaces.REST.Resources;

namespace OsitoPolar.Analytics.Service.Interfaces.REST.Transform;

/// <summary>
/// Assembles a RecordEnergyReadingCommand from a CreateEnergyReadingResource
/// </summary>
public static class CreateEnergyReadingCommandFromResourceAssembler
{
    public static RecordEnergyReadingCommand ToCommandFromResource(CreateEnergyReadingResource resource)
    {
        return new RecordEnergyReadingCommand(
            resource.EquipmentId,
            resource.Consumption,
            resource.Unit
        );
    }
}