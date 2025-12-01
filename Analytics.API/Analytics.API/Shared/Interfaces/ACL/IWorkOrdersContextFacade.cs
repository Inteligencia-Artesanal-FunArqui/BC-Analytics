namespace OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

/// <summary>
/// Facade for communicating with WorkOrders Service
/// Analytics Service only needs READ operations to gather work order metrics
/// </summary>
public interface IWorkOrdersContextFacade
{
    /// <summary>
    /// Get total count of all work orders
    /// </summary>
    /// <returns>Total work order count</returns>
    Task<int> GetTotalWorkOrdersCountAsync();

    /// <summary>
    /// Get work order count by status
    /// </summary>
    /// <param name="status">Work order status (Pending, InProgress, Completed, Cancelled)</param>
    /// <returns>Work order count for the specified status</returns>
    Task<int> GetWorkOrdersCountByStatusAsync(string status);

    /// <summary>
    /// Get work order count by technician
    /// </summary>
    /// <param name="technicianId">Technician ID</param>
    /// <returns>Work order count for the specified technician</returns>
    Task<int> GetWorkOrdersCountByTechnicianAsync(int technicianId);

    /// <summary>
    /// Get work order count by equipment
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>Work order count for the specified equipment</returns>
    Task<int> GetWorkOrdersCountByEquipmentAsync(int equipmentId);

    /// <summary>
    /// Get work orders statistics summary
    /// </summary>
    /// <returns>Work orders statistics DTO</returns>
    Task<WorkOrdersStatsDto> GetWorkOrdersStatsAsync();

    /// <summary>
    /// Get average completion time for work orders (in hours)
    /// </summary>
    /// <param name="startDate">Start date for the period</param>
    /// <param name="endDate">End date for the period</param>
    /// <returns>Average completion time in hours</returns>
    Task<double> GetAverageCompletionTimeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get work orders created in a date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>Work order count for the date range</returns>
    Task<int> GetWorkOrdersCountByDateRangeAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// DTO for WorkOrders statistics
/// </summary>
public record WorkOrdersStatsDto(
    int TotalWorkOrders,
    int PendingWorkOrders,
    int InProgressWorkOrders,
    int CompletedWorkOrders,
    int CancelledWorkOrders,
    double AverageCompletionTimeHours,
    Dictionary<string, int> WorkOrdersByPriority
);
