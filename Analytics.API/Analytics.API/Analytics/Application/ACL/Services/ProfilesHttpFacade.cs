using System.Net.Http.Json;
using OsitoPolar.Analytics.Service.Shared.Interfaces.ACL;

namespace OsitoPolar.Analytics.Service.Application.ACL.Services;

/// <summary>
/// HTTP Facade for communicating with Profiles Service
/// Provides READ-ONLY access to profile data for analytics purposes
/// </summary>
public class ProfilesHttpFacade : IProfilesContextFacade
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProfilesHttpFacade> _logger;

    public ProfilesHttpFacade(HttpClient httpClient, ILogger<ProfilesHttpFacade> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total users count from Profiles Service");

            var response = await _httpClient.GetAsync("/api/v1/profiles/stats/total-users");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total users count: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<ProfilesCountResponse>();
            return result?.Count ?? 0;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching total users count");
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching total users count");
            return 0;
        }
    }

    public async Task<int> GetTotalOwnersCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total owners count from Profiles Service");

            var response = await _httpClient.GetAsync("/api/v1/profiles/stats/total-owners");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total owners count: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<ProfilesCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching total owners count");
            return 0;
        }
    }

    public async Task<int> GetTotalProvidersCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total providers count from Profiles Service");

            var response = await _httpClient.GetAsync("/api/v1/profiles/stats/total-providers");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total providers count: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<ProfilesCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching total providers count");
            return 0;
        }
    }

    public async Task<int> GetTotalTechniciansCountAsync()
    {
        try
        {
            _logger.LogInformation("Fetching total technicians count from Profiles Service");

            var response = await _httpClient.GetAsync("/api/v1/profiles/stats/total-technicians");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch total technicians count: {StatusCode}", response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<ProfilesCountResponse>();
            return result?.Count ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching total technicians count");
            return 0;
        }
    }

    public async Task<OwnerDto?> GetOwnerByIdAsync(int ownerId)
    {
        try
        {
            _logger.LogInformation("Fetching Owner {OwnerId} from Profiles Service", ownerId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/owners/{ownerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch owner {OwnerId}: {StatusCode}", ownerId, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<OwnerResponse>();

            if (result == null)
                return null;

            return new OwnerDto(
                result.Id,
                result.UserId,
                result.Name,
                result.Email,
                result.PlanId,
                result.MaxUnits
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching owner {OwnerId}", ownerId);
            return null;
        }
    }

    public async Task<RenterProviderDto?> GetProviderByIdAsync(int providerId)
    {
        try
        {
            _logger.LogInformation("Fetching Provider {ProviderId} from Profiles Service", providerId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/providers/{providerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch provider {ProviderId}: {StatusCode}", providerId, response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<RenterProviderResponse>();

            if (result == null)
                return null;

            return new RenterProviderDto(
                result.Id,
                result.UserId,
                result.Name,
                result.Email,
                result.CompanyName,
                result.PlanId,
                result.MaxClients
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching provider {ProviderId}", providerId);
            return null;
        }
    }

    public async Task<int> FetchOwnerIdByUserId(int userId)
    {
        try
        {
            _logger.LogInformation("Fetching Owner ID for user {UserId} from Profiles Service", userId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/owners/by-user/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No Owner profile found for user {UserId}", userId);
                return 0;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch Owner ID for user {UserId}: {StatusCode}", userId, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<OwnerResponse>();

            return result?.Id ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching Owner ID for user {UserId}", userId);
            return 0;
        }
    }

    public async Task<int> FetchProviderIdByUserId(int userId)
    {
        try
        {
            _logger.LogInformation("Fetching Provider ID for user {UserId} from Profiles Service", userId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/providers/by-user/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("No Provider profile found for user {UserId}", userId);
                return 0;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to fetch Provider ID for user {UserId}: {StatusCode}", userId, response.StatusCode);
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<RenterProviderResponse>();

            return result?.Id ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching Provider ID for user {UserId}", userId);
            return 0;
        }
    }
}

// Response DTOs for HTTP communication
internal record ProfilesCountResponse(int Count);

internal record OwnerResponse(
    int Id,
    int UserId,
    string Name,
    string Email,
    int PlanId,
    int MaxUnits
);

internal record RenterProviderResponse(
    int Id,
    int UserId,
    string Name,
    string Email,
    string CompanyName,
    int PlanId,
    int MaxClients
);
