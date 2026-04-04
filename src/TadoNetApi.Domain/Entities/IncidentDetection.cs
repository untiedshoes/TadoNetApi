namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the incident detection settings for a home.
/// </summary>
public class IncidentDetection
{
    /// <summary>
    /// Indicates whether incident detection is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Indicates whether incident detection is supported for the home.
    /// </summary>
    public bool? Supported { get; set; }
}