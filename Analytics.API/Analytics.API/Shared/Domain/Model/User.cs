namespace OsitoPolar.Analytics.Service.Shared.Domain.Model;

/// <summary>
/// Simplified User model for Analytics Service
/// Full authentication is handled by IAM Service
/// This is a lightweight DTO for passing user context
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public User() { }

    public User(int id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }
}
