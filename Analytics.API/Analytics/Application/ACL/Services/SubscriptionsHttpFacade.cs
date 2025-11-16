using System.Net.Http.Json;
using OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

namespace OsitoPolar.Analytics.Service.Application.ACL.Services;

/// <summary>
/// HTTP Facade for communicating with Subscriptions Service
/// Provides READ-ONLY access to subscription and revenue data for analytics purposes
/// </summary>
public class SubscriptionsHttpFacade : ISubscriptionsContextFacade
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SubscriptionsHttpFacade> _logger;

    public SubscriptionsHttpFacade(HttpClient httpClient, ILogger<SubscriptionsHttpFacade> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int> GetTotalActiveSubscriptionsCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total active subscriptions count from Subscriptions Service");

            var response = await _httpClient.GetAsync("/api/v1/subscriptions/stats/total-active");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total active subscriptions count: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<SubscriptionsCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching total active subscriptions count");
            return 0;
        }
    }

    public async Task<int> GetSubscriptionCountByPlanAsync(string planName)
    {
        try
        {
            _logger.LogInformation("Fetching subscription count for plan {PlanName} from Subscriptions Service", planName);

            var response = await _httpClient.GetAsync($"/api/v1/subscriptions/stats/by-plan/{planName}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch subscription count for plan {PlanName}: {StatusCode}", planName, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<SubscriptionsCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching subscription count for plan {PlanName}", planName);
            return 0;
        }
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Fetching total revenue from {StartDate} to {EndDate} from Subscriptions Service", startDate, endDate);

            var response = await _httpClient.GetAsync(
                $"/api/v1/subscriptions/stats/revenue?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total revenue: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<RevenueResponse>();
            return result?.TotalRevenue ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching total revenue");
            return 0;
        }
    }

    public async Task<decimal> GetMonthlyRecurringRevenueAsync()
    {
        try
        {
            _logger.LogInformation("Fetching monthly recurring revenue from Subscriptions Service");

            var response = await _httpClient.GetAsync("/api/v1/subscriptions/stats/mrr");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch monthly recurring revenue: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<MrrResponse>();
            return result?.MonthlyRecurringRevenue ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching monthly recurring revenue");
            return 0;
        }
    }

    public async Task<RevenueStatsDto> GetRevenueStatsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching revenue statistics from Subscriptions Service");

            var response = await _httpClient.GetAsync("/api/v1/subscriptions/stats/revenue-summary");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch revenue statistics: {StatusCode}", response.StatusCode);
                return new RevenueStatsDto(0, 0, 0, new Dictionary<string, int>(), new Dictionary<string, decimal>());
            }

            var result = await response.Content.ReadFromJsonAsync<RevenueStatsResponse>();

            if (result == null)
                return new RevenueStatsDto(0, 0, 0, new Dictionary<string, int>(), new Dictionary<string, decimal>());

            return new RevenueStatsDto(
                result.TotalRevenue,
                result.MonthlyRecurringRevenue,
                result.TotalActiveSubscriptions,
                result.SubscriptionsByPlan ?? new Dictionary<string, int>(),
                result.RevenueByPlan ?? new Dictionary<string, decimal>()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching revenue statistics");
            return new RevenueStatsDto(0, 0, 0, new Dictionary<string, int>(), new Dictionary<string, decimal>());
        }
    }

    public async Task<SubscriptionPlanDto?> GetSubscriptionPlanByIdAsync(int planId)
    {
        try
        {
            _logger.LogInformation("Fetching subscription plan {PlanId} from Subscriptions Service", planId);

            var response = await _httpClient.GetAsync($"/api/v1/subscriptions/plans/{planId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch subscription plan {PlanId}: {StatusCode}", planId, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<SubscriptionPlanResponse>();

            if (result == null)
                return null;

            return new SubscriptionPlanDto(
                result.Id,
                result.PlanName,
                result.Price,
                result.MaxClients,
                result.MaxEquipment,
                result.IsActive
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching subscription plan {PlanId}", planId);
            return null;
        }
    }

    public async Task<List<SubscriptionPlanDto>> GetAllSubscriptionPlansAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all subscription plans from Subscriptions Service");

            var response = await _httpClient.GetAsync("/api/v1/subscriptions/plans");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch all subscription plans: {StatusCode}", response.StatusCode);
                return new List<SubscriptionPlanDto>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<SubscriptionPlanResponse>>();

            if (result == null)
                return new List<SubscriptionPlanDto>();

            return result.Select(r => new SubscriptionPlanDto(
                r.Id,
                r.PlanName,
                r.Price,
                r.MaxClients,
                r.MaxEquipment,
                r.IsActive
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching all subscription plans");
            return new List<SubscriptionPlanDto>();
        }
    }
}

// Response DTOs for HTTP communication
internal record SubscriptionsCountResponse(int Count);

internal record RevenueResponse(decimal TotalRevenue);

internal record MrrResponse(decimal MonthlyRecurringRevenue);

internal record RevenueStatsResponse(
    decimal TotalRevenue,
    decimal MonthlyRecurringRevenue,
    int TotalActiveSubscriptions,
    Dictionary<string, int>? SubscriptionsByPlan,
    Dictionary<string, decimal>? RevenueByPlan
);

internal record SubscriptionPlanResponse(
    int Id,
    string PlanName,
    decimal Price,
    int MaxClients,
    int MaxEquipment,
    bool IsActive
);
