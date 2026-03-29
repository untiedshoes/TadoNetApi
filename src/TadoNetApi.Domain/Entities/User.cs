namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a Tado user associated with a home.
/// </summary>
public class User
{
    /// <summary>
    /// Gets the unique identifier of the user.
    /// </summary>
    public string  Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets the name of the user.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary> 
    /// Gets the email address of the user.
    /// </summary>  
    public string? Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets the list of homes associated with the user.
    /// </summary>
    public List<Home> Homes { get; set; } = new List<Home>();

    /// <summary>
    /// Gets the locale of the user (e.g., "en_GB").
    /// </summary>
    public string? Locale { get; set; }
}