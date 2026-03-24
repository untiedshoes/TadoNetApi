namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the current state of a zone.
/// </summary>
public class ZoneState
{
    /// <summary>Current temperature in the zone.</summary>
    public double Temperature { get; set; }

    /// <summary>Current humidity in the zone.</summary>
    public double Humidity { get; set; }

    /// <summary>Power state of the zone (ON/OFF).</summary>
    public string Power { get; set; } = string.Empty;

    /// <summary>Overlay type currently applied (if any).</summary>
    public string OverlayType { get; set; } = string.Empty;

    /// <summary>Type of overlay target temperature.</summary>
    public string OverlayTargetTemperatureType { get; set; } = string.Empty;
}