namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a Tado user associated with a home.
/// </summary>
public class User
{
    /// <summary>User's unique identifier.</summary>
    public int Id { get; set; }

    /// <summary>User's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>User's display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Identifier of the home associated with the user.</summary>
    public int HomeId { get; set; }

    /// <summary>User's locale (e.g., "en-US").</summary>
    public string? Locale { get; set; }
}