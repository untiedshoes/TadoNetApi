namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a Tado Home (building) containing zones and devices.
/// Mirrors the Home DTO from the Tado API.
/// </summary>
public class Home
{
    /// <summary>
    /// The unique identifier of the home.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The full name of the home.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The Timezone of the home.
    /// </summary>
    public string? Timezone { get; set; }
}