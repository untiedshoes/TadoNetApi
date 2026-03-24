namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the current weather conditions for a home.
/// </summary>
public class Weather
{
    /// <summary>Current temperature in Celsius.</summary>
    public double Temperature { get; set; }

    /// <summary>Current humidity as a percentage.</summary>
    public double Humidity { get; set; }

    /// <summary>Wind speed in m/s.</summary>
    public double WindSpeed { get; set; }

    /// <summary>Rain volume in mm (if any).</summary>
    public double Rain { get; set; }

    /// <summary>Weather description or condition (e.g., Sunny, Cloudy).</summary>
    public string Condition { get; set; } = string.Empty;
}