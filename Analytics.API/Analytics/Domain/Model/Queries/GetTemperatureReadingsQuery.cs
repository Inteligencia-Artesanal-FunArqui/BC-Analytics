namespace OsitoPolar.Analytics.Service.Domain.Model.Queries;

public record GetTemperatureReadingsQuery(int EquipmentId, int Hours = 24);