namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a Tado Home (building) containing zones and devices.
/// Mirrors the Home DTO from the Tado API.
/// </summary>
public class Home
{
    /// <summary>
    /// The unique identifier of the home
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// The name of the home
    /// </summary>
    public string? Name { get; set; }
}