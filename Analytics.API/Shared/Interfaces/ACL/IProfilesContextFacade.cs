namespace OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

/// <summary>
/// Facade for communicating with Profiles Service
/// Analytics Service only needs READ operations to gather user metrics
/// </summary>
public interface IProfilesContextFacade
{
    /// <summary>
    /// Get total count of all users (Owners + RenterProviders + Technicians)
    /// </summary>
    /// <returns>Total user count</returns>
    Task<int> GetTotalUsersCountAsync();

    /// <summary>
    /// Get total count of Owner profiles
    /// </summary>
    /// <returns>Owner count</returns>
    Task<int> GetTotalOwnersCountAsync();

    /// <summary>
    /// Get total count of RenterProvider profiles
    /// </summary>
    /// <returns>RenterProvider count</returns>
    Task<int> GetTotalProvidersCountAsync();

    /// <summary>
    /// Get total count of Technician profiles
    /// </summary>
    /// <returns>Technician count</returns>
    Task<int> GetTotalTechniciansCountAsync();

    /// <summary>
    /// Get Owner profile by ID
    /// </summary>
    /// <param name="ownerId">Owner ID</param>
    /// <returns>Owner DTO or null if not found</returns>
    Task<OwnerDto?> GetOwnerByIdAsync(int ownerId);

    /// <summary>
    /// Get RenterProvider profile by ID
    /// </summary>
    /// <param name="providerId">Provider ID</param>
    /// <returns>Provider DTO or null if not found</returns>
    Task<RenterProviderDto?> GetProviderByIdAsync(int providerId);

    /// <summary>
    /// Fetch Owner ID by User ID (used by controllers for authentication)
    /// </summary>
    /// <param name="userId">User ID from authentication</param>
    /// <returns>Owner ID if found, 0 otherwise</returns>
    Task<int> FetchOwnerIdByUserId(int userId);

    /// <summary>
    /// Fetch Provider ID by User ID (used by controllers for authentication)
    /// </summary>
    /// <param name="userId">User ID from authentication</param>
    /// <returns>Provider ID if found, 0 otherwise</returns>
    Task<int> FetchProviderIdByUserId(int userId);
}

/// <summary>
/// DTO for Owner profile data
/// </summary>
public record OwnerDto(
    int Id,
    int UserId,
    string Name,
    string Email,
    int PlanId,
    int MaxUnits
);

/// <summary>
/// DTO for RenterProvider profile data
/// </summary>
public record RenterProviderDto(
    int Id,
    int UserId,
    string Name,
    string Email,
    string CompanyName,
    int PlanId,
    int MaxClients
);
