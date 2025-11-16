namespace OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

/// <summary>
/// Facade for communicating with Equipment Service
/// Analytics Service only needs READ operations to gather equipment metrics
/// </summary>
public interface IEquipmentContextFacade
{
    /// <summary>
    /// Get total count of all equipment
    /// </summary>
    /// <returns>Total equipment count</returns>
    Task<int> GetTotalEquipmentCountAsync();

    /// <summary>
    /// Get equipment count by status
    /// </summary>
    /// <param name="status">Equipment status (Available, InUse, UnderMaintenance, etc.)</param>
    /// <returns>Equipment count for the specified status</returns>
    Task<int> GetEquipmentCountByStatusAsync(string status);

    /// <summary>
    /// Get equipment count by owner
    /// </summary>
    /// <param name="ownerId">Owner ID</param>
    /// <returns>Equipment count for the specified owner</returns>
    Task<int> GetEquipmentCountByOwnerAsync(int ownerId);

    /// <summary>
    /// Get equipment by ID
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>Equipment DTO or null if not found</returns>
    Task<EquipmentDto?> GetEquipmentByIdAsync(int equipmentId);

    /// <summary>
    /// Get all equipment IDs (for analytics processing)
    /// </summary>
    /// <returns>List of all equipment IDs</returns>
    Task<List<int>> GetAllEquipmentIdsAsync();

    /// <summary>
    /// Get equipment statistics summary
    /// </summary>
    /// <returns>Equipment statistics DTO</returns>
    Task<EquipmentStatsDto> GetEquipmentStatsAsync();

    /// <summary>
    /// Check if equipment exists
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>True if equipment exists, false otherwise</returns>
    Task<bool> EquipmentExists(int equipmentId);

    /// <summary>
    /// Check if equipment is owned by a specific owner
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <param name="ownerId">Owner ID</param>
    /// <returns>True if equipment is owned by the owner, false otherwise</returns>
    Task<bool> IsEquipmentOwnedBy(int equipmentId, int ownerId);

    /// <summary>
    /// Fetch all equipment IDs owned by an owner
    /// </summary>
    /// <param name="ownerId">Owner ID</param>
    /// <returns>List of equipment IDs</returns>
    Task<IEnumerable<int>> FetchEquipmentIdsByOwnerId(int ownerId);

    /// <summary>
    /// Get equipment optimal temperature range
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>Tuple with (minTemp, maxTemp) or null if not found</returns>
    Task<(decimal minTemp, decimal maxTemp)?> GetEquipmentOptimalTemperatureRange(int equipmentId);

    /// <summary>
    /// Get equipment maintenance data for forecasting
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>Tuple with (installationDate, minTemp, maxTemp) or null if not found</returns>
    Task<(DateTimeOffset installationDate, decimal minTemp, decimal maxTemp)?> GetEquipmentMaintenanceData(int equipmentId);
}

/// <summary>
/// DTO for Equipment data
/// </summary>
public record EquipmentDto(
    int Id,
    string Name,
    string Type,
    string Status,
    int OwnerId,
    decimal? OptimalTemperatureMin,
    decimal? OptimalTemperatureMax,
    decimal? EnergyConsumptionRating
);

/// <summary>
/// DTO for Equipment statistics
/// </summary>
public record EquipmentStatsDto(
    int TotalEquipment,
    int AvailableEquipment,
    int InUseEquipment,
    int UnderMaintenanceEquipment,
    Dictionary<string, int> EquipmentByType
);
