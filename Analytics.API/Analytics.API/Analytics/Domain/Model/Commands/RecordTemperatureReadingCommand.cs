namespace OsitoPolar.Analytics.Service.Domain.Model.Commands;

public record RecordTemperatureReadingCommand(
    int EquipmentId,
    decimal Temperature,
    DateTimeOffset? Timestamp = null
);