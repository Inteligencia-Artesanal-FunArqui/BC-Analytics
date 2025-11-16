using System.Net.Http.Json;
using OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

namespace OsitoPolar.Analytics.Service.Application.ACL.Services;

/// <summary>
/// HTTP Facade for communicating with WorkOrders Service
/// Provides READ-ONLY access to work order data for analytics purposes
/// </summary>
public class WorkOrdersHttpFacade : IWorkOrdersContextFacade
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkOrdersHttpFacade> _logger;

    public WorkOrdersHttpFacade(HttpClient httpClient, ILogger<WorkOrdersHttpFacade> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int> GetTotalWorkOrdersCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total work orders count from WorkOrders Service");

            var response = await _httpClient.GetAsync("/api/v1/work-orders/stats/total");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total work orders count: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<WorkOrdersCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching total work orders count");
            return 0;
        }
    }

    public async Task<int> GetWorkOrdersCountByStatusAsync(string status)
    {
        try
        {
            _logger.LogInformation("Fetching work orders count by status {Status} from WorkOrders Service", status);

            var response = await _httpClient.GetAsync($"/api/v1/work-orders/stats/by-status/{status}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch work orders count by status {Status}: {StatusCode}", status, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<WorkOrdersCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching work orders count by status {Status}", status);
            return 0;
        }
    }

    public async Task<int> GetWorkOrdersCountByTechnicianAsync(int technicianId)
    {
        try
        {
            _logger.LogInformation("Fetching work orders count for technician {TechnicianId} from WorkOrders Service", technicianId);

            var response = await _httpClient.GetAsync($"/api/v1/work-orders/stats/by-technician/{technicianId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch work orders count for technician {TechnicianId}: {StatusCode}", technicianId, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<WorkOrdersCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching work orders count for technician {TechnicianId}", technicianId);
            return 0;
        }
    }

    public async Task<int> GetWorkOrdersCountByEquipmentAsync(int equipmentId)
    {
        try
        {
            _logger.LogInformation("Fetching work orders count for equipment {EquipmentId} from WorkOrders Service", equipmentId);

            var response = await _httpClient.GetAsync($"/api/v1/work-orders/stats/by-equipment/{equipmentId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch work orders count for equipment {EquipmentId}: {StatusCode}", equipmentId, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<WorkOrdersCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching work orders count for equipment {EquipmentId}", equipmentId);
            return 0;
        }
    }

    public async Task<WorkOrdersStatsDto> GetWorkOrdersStatsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching work orders statistics from WorkOrders Service");

            var response = await _httpClient.GetAsync("/api/v1/work-orders/stats");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch work orders statistics: {StatusCode}", response.StatusCode);
                return new WorkOrdersStatsDto(0, 0, 0, 0, 0, 0, new Dictionary<string, int>());
            }

            var result = await response.Content.ReadFromJsonAsync<WorkOrdersStatsResponse>();

            if (result == null)
                return new WorkOrdersStatsDto(0, 0, 0, 0, 0, 0, new Dictionary<string, int>());

            return new WorkOrdersStatsDto(
                result.TotalWorkOrders,
                result.PendingWorkOrders,
                result.InProgressWorkOrders,
                result.CompletedWorkOrders,
                result.CancelledWorkOrders,
                result.AverageCompletionTimeHours,
                result.WorkOrdersByPriority ?? new Dictionary<string, int>()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching work orders statistics");
            return new WorkOrdersStatsDto(0, 0, 0, 0, 0, 0, new Dictionary<string, int>());
        }
    }

    public async Task<double> GetAverageCompletionTimeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Fetching average completion time from {StartDate} to {EndDate} from WorkOrders Service", startDate, endDate);

            var response = await _httpClient.GetAsync(
                $"/api/v1/work-orders/stats/average-completion-time?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch average completion time: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<AverageTimeResponse>();
            return result?.AverageHours ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching average completion time");
            return 0;
        }
    }

    public async Task<int> GetWorkOrdersCountByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Fetching work orders count from {StartDate} to {EndDate} from WorkOrders Service", startDate, endDate);

            var response = await _httpClient.GetAsync(
                $"/api/v1/work-orders/stats/by-date-range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch work orders count by date range: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<WorkOrdersCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching work orders count by date range");
            return 0;
        }
    }
}

// Response DTOs for HTTP communication
internal record WorkOrdersCountResponse(int Count);

internal record AverageTimeResponse(double AverageHours);

internal record WorkOrdersStatsResponse(
    int TotalWorkOrders,
    int PendingWorkOrders,
    int InProgressWorkOrders,
    int CompletedWorkOrders,
    int CancelledWorkOrders,
    double AverageCompletionTimeHours,
    Dictionary<string, int>? WorkOrdersByPriority
);
