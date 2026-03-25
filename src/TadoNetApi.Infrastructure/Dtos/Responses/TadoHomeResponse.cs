using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the raw Home response from the Tado API.
/// Mirrors the JSON structure returned by the API.
/// </summary>
public class TadoHomeResponse
{
    /// <summary>The unique identifier of the home.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>The full name of the home.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>The short name of the home.</summary>
    [JsonPropertyName("shortName")]
    public string? ShortName { get; set; }

    /// <summary>The timezone of the home.</summary>
    [JsonPropertyName("timeZone")]
    public string? TimeZone { get; set; }

    /// <summary>The country code of the home.</summary>
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    /// <summary>Indicates if the home is active.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>The API version.</summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>Indicates if open window detection is enabled.</summary>
    [JsonPropertyName("openWindowDetectionEnabled")]
    public bool? OpenWindowDetectionEnabled { get; set; }
}