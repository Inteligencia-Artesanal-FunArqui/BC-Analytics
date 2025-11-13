namespace OsitoPolar.Analytics.Service.Domain.Model.Queries;

public record GetEnergyReadingsQuery(int EquipmentId, int Hours = 24);