namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a Tado Home (building) containing zones and devices.
/// Mirrors the Home DTO from the Tado API.
/// </summary>
public class Home
{
    /// <summary>The unique identifier of the home.</summary>
    public int Id { get; set; }

    /// <summary>The full name of the home.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The short name of the home.</summary>
    public string ShortName { get; set; } = string.Empty;

    /// <summary>The address of the home.</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Current presence state of the home (e.g., HOME, AWAY).</summary>
    public string Presence { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if open window detection is enabled.
    /// </summary>
    public bool? OpenWindowDetectionEnabled { get; set; }

    /// <summary>Collection of zones within the home.</summary>
    public List<Zone> Zones { get; set; } = new();

    public string CountryCode { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}