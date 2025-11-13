namespace OsitoPolar.Analytics.Service.Domain.Model.Queries;

public record GetDailyTemperatureAveragesQuery(int EquipmentId, int Days = 7);