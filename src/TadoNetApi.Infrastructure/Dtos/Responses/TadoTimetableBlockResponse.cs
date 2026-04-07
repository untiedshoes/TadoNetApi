using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents a timetable block returned by the Tado API.
/// </summary>
public class TadoTimetableBlockResponse
{
    [JsonPropertyName("dayType")]
    public string? DayType { get; set; }

    [JsonPropertyName("start")]
    public string? Start { get; set; }

    [JsonPropertyName("end")]
    public string? End { get; set; }

    [JsonPropertyName("geolocationOverride")]
    public bool? GeolocationOverride { get; set; }

    [JsonPropertyName("setting")]
    public TadoSettingResponse? Setting { get; set; }
}