namespace OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

/// <summary>
/// Facade for communicating with Subscriptions Service
/// Analytics Service only needs READ operations to gather subscription and revenue metrics
/// </summary>
public interface ISubscriptionsContextFacade
{
    /// <summary>
    /// Get total count of active subscriptions
    /// </summary>
    /// <returns>Active subscription count</returns>
    Task<int> GetTotalActiveSubscriptionsCountAsync();

    /// <summary>
    /// Get subscription count by plan name
    /// </summary>
    /// <param name="planName">Plan name (Free, Basic, Premium, Enterprise)</param>
    /// <returns>Subscription count for the specified plan</returns>
    Task<int> GetSubscriptionCountByPlanAsync(string planName);

    /// <summary>
    /// Get total revenue for a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Total revenue amount</returns>
    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get monthly recurring revenue (MRR)
    /// </summary>
    /// <returns>Monthly recurring revenue</returns>
    Task<decimal> GetMonthlyRecurringRevenueAsync();

    /// <summary>
    /// Get revenue statistics summary
    /// </summary>
    /// <returns>Revenue statistics DTO</returns>
    Task<RevenueStatsDto> GetRevenueStatsAsync();

    /// <summary>
    /// Get subscription plan by ID
    /// </summary>
    /// <param name="planId">Plan ID</param>
    /// <returns>Subscription plan DTO or null if not found</returns>
    Task<SubscriptionPlanDto?> GetSubscriptionPlanByIdAsync(int planId);

    /// <summary>
    /// Get all available subscription plans
    /// </summary>
    /// <returns>List of subscription plan DTOs</returns>
    Task<List<SubscriptionPlanDto>> GetAllSubscriptionPlansAsync();
}

/// <summary>
/// DTO for Revenue statistics
/// </summary>
public record RevenueStatsDto(
    decimal TotalRevenue,
    decimal MonthlyRecurringRevenue,
    int TotalActiveSubscriptions,
    Dictionary<string, int> SubscriptionsByPlan,
    Dictionary<string, decimal> RevenueByPlan
);

/// <summary>
/// DTO for Subscription Plan data
/// </summary>
public record SubscriptionPlanDto(
    int Id,
    string PlanName,
    decimal Price,
    int MaxClients,
    int MaxEquipment,
    bool IsActive
);
