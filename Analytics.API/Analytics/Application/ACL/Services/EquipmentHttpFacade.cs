using System.Net.Http.Json;
using OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

namespace OsitoPolar.Analytics.Service.Application.ACL.Services;

/// <summary>
/// HTTP Facade for communicating with Equipment Service
/// Provides READ-ONLY access to equipment data for analytics purposes
/// </summary>
public class EquipmentHttpFacade : IEquipmentContextFacade
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EquipmentHttpFacade> _logger;

    public EquipmentHttpFacade(HttpClient httpClient, ILogger<EquipmentHttpFacade> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int> GetTotalEquipmentCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total equipment count from Equipment Service");

            var response = await _httpClient.GetAsync("/api/v1/equipment/stats/total");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total equipment count: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<EquipmentCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching total equipment count");
            return 0;
        }
    }

    public async Task<int> GetEquipmentCountByStatusAsync(string status)
    {
        try
        {
            _logger.LogInformation("Fetching equipment count by status {Status} from Equipment Service", status);

            var response = await _httpClient.GetAsync($"/api/v1/equipment/stats/by-status/{status}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch equipment count by status {Status}: {StatusCode}", status, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<EquipmentCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching equipment count by status {Status}", status);
            return 0;
        }
    }

    public async Task<int> GetEquipmentCountByOwnerAsync(int ownerId)
    {
        try
        {
            _logger.LogInformation("Fetching equipment count for owner {OwnerId} from Equipment Service", ownerId);

            var response = await _httpClient.GetAsync($"/api/v1/equipment/stats/by-owner/{ownerId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch equipment count for owner {OwnerId}: {StatusCode}", ownerId, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<EquipmentCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching equipment count for owner {OwnerId}", ownerId);
            return 0;
        }
    }

    public async Task<EquipmentDto?> GetEquipmentByIdAsync(int equipmentId)
    {
        try
        {
            _logger.LogInformation("Fetching Equipment {EquipmentId} from Equipment Service", equipmentId);

            var response = await _httpClient.GetAsync($"/api/v1/equipment/{equipmentId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch equipment {EquipmentId}: {StatusCode}", equipmentId, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<EquipmentResponse>();

            if (result == null)
                return null;

            return new EquipmentDto(
                result.Id,
                result.Name,
                result.Type,
                result.Status,
                result.OwnerId,
                result.OptimalTemperatureMin,
                result.OptimalTemperatureMax,
                result.EnergyConsumptionRating
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching equipment {EquipmentId}", equipmentId);
            return null;
        }
    }

    public async Task<List<int>> GetAllEquipmentIdsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all equipment IDs from Equipment Service");

            var response = await _httpClient.GetAsync("/api/v1/equipment/stats/all-ids");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch all equipment IDs: {StatusCode}", response.StatusCode);
                return new List<int>();
            }

            var result = await response.Content.ReadFromJsonAsync<IdsResponse>();
            return result?.Ids ?? new List<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching all equipment IDs");
            return new List<int>();
        }
    }

    public async Task<EquipmentStatsDto> GetEquipmentStatsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching equipment statistics from Equipment Service");

            var response = await _httpClient.GetAsync("/api/v1/equipment/stats");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch equipment statistics: {StatusCode}", response.StatusCode);
                return new EquipmentStatsDto(0, 0, 0, 0, new Dictionary<string, int>());
            }

            var result = await response.Content.ReadFromJsonAsync<EquipmentStatsResponse>();

            if (result == null)
                return new EquipmentStatsDto(0, 0, 0, 0, new Dictionary<string, int>());

            return new EquipmentStatsDto(
                result.TotalEquipment,
                result.AvailableEquipment,
                result.InUseEquipment,
                result.UnderMaintenanceEquipment,
                result.EquipmentByType ?? new Dictionary<string, int>()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching equipment statistics");
            return new EquipmentStatsDto(0, 0, 0, 0, new Dictionary<string, int>());
        }
    }

    public async Task<bool> EquipmentExists(int equipmentId)
    {
        try
        {
            _logger.LogInformation("Checking if equipment {EquipmentId} exists in Equipment Service", equipmentId);

            var response = await _httpClient.GetAsync($"/api/v1/equipment/{equipmentId}/exists");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to check if equipment {EquipmentId} exists: {StatusCode}", equipmentId, response.StatusCode);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ExistsResponse>();
            return result?.Exists ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking if equipment {EquipmentId} exists", equipmentId);
            return false;
        }
    }

    public async Task<bool> IsEquipmentOwnedBy(int equipmentId, int ownerId)
    {
        try
        {
            _logger.LogInformation("Checking if equipment {EquipmentId} is owned by owner {OwnerId} in Equipment Service", equipmentId, ownerId);

            var response = await _httpClient.GetAsync($"/api/v1/equipment/{equipmentId}/is-owned-by/{ownerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to check equipment ownership for {EquipmentId} and owner {OwnerId}: {StatusCode}", equipmentId, ownerId, response.StatusCode);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<OwnershipResponse>();
            return result?.IsOwned ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking equipment ownership for {EquipmentId} and owner {OwnerId}", equipmentId, ownerId);
            return false;
        }
    }

    public async Task<IEnumerable<int>> FetchEquipmentIdsByOwnerId(int ownerId)
    {
        try
        {
            _logger.LogInformation("Fetching equipment IDs for owner {OwnerId} from Equipment Service", ownerId);

            // Call Equipment's EquipmentContextFacade directly via a dedicated endpoint
            var response = await _httpClient.GetAsync($"/api/v1/equipment/acl/by-owner/{ownerId}/ids");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch equipment IDs for owner {OwnerId}: {StatusCode}", ownerId, response.StatusCode);
                return Enumerable.Empty<int>();
            }

            var result = await response.Content.ReadFromJsonAsync<IdsResponse>();
            return result?.Ids ?? Enumerable.Empty<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching equipment IDs for owner {OwnerId}", ownerId);
            return Enumerable.Empty<int>();
        }
    }

    public async Task<(decimal minTemp, decimal maxTemp)?> GetEquipmentOptimalTemperatureRange(int equipmentId)
    {
        try
        {
            _logger.LogInformation("Fetching optimal temperature range for equipment {EquipmentId} from Equipment Service", equipmentId);

            var response = await _httpClient.GetAsync($"/api/v1/equipment/acl/{equipmentId}/temperature-range");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch temperature range for equipment {EquipmentId}: {StatusCode}", equipmentId, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<TemperatureRangeResponse>();
            if (result == null) return null;

            return (result.MinTemp, result.MaxTemp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching temperature range for equipment {EquipmentId}", equipmentId);
            return null;
        }
    }

    public async Task<(DateTimeOffset installationDate, decimal minTemp, decimal maxTemp)?> GetEquipmentMaintenanceData(int equipmentId)
    {
        try
        {
            _logger.LogInformation("Fetching maintenance data for equipment {EquipmentId} from Equipment Service", equipmentId);

            var response = await _httpClient.GetAsync($"/api/v1/equipment/acl/{equipmentId}/maintenance-data");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch maintenance data for equipment {EquipmentId}: {StatusCode}", equipmentId, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<MaintenanceDataResponse>();
            if (result == null) return null;

            return (result.InstallationDate, result.MinTemp, result.MaxTemp);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching maintenance data for equipment {EquipmentId}", equipmentId);
            return null;
        }
    }
}

// Response DTOs for HTTP communication
internal record EquipmentCountResponse(int Count);

internal record IdsResponse(List<int> Ids);

internal record EquipmentResponse(
    int Id,
    string Name,
    string Type,
    string Status,
    int OwnerId,
    decimal? OptimalTemperatureMin,
    decimal? OptimalTemperatureMax,
    decimal? EnergyConsumptionRating
);

internal record EquipmentStatsResponse(
    int TotalEquipment,
    int AvailableEquipment,
    int InUseEquipment,
    int UnderMaintenanceEquipment,
    Dictionary<string, int>? EquipmentByType
);

internal record ExistsResponse(bool Exists);

internal record OwnershipResponse(bool IsOwned);

internal record TemperatureRangeResponse(decimal MinTemp, decimal MaxTemp);

internal record MaintenanceDataResponse(DateTimeOffset InstallationDate, decimal MinTemp, decimal MaxTemp);
