using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating timetable blocks for a specific day type.
/// </summary>
public sealed class SetTimetableBlockRequest
{
    [JsonPropertyName("dayType")]
    public string DayType { get; set; } = string.Empty;

    [JsonPropertyName("start")]
    public string Start { get; set; } = string.Empty;

    [JsonPropertyName("end")]
    public string End { get; set; } = string.Empty;

    [JsonPropertyName("geolocationOverride")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? GeolocationOverride { get; set; }

    [JsonPropertyName("setting")]
    public SetZoneTemperatureSettingRequest Setting { get; set; } = new();

    /// <summary>
    /// Maps a domain <see cref="TimetableBlock"/> into the API request shape.
    /// </summary>
    public static SetTimetableBlockRequest FromDomain(TimetableBlock timetableBlock)
    {
        ArgumentNullException.ThrowIfNull(timetableBlock);

        return new SetTimetableBlockRequest
        {
            DayType = timetableBlock.DayType ?? string.Empty,
            Start = timetableBlock.Start ?? string.Empty,
            End = timetableBlock.End ?? string.Empty,
            GeolocationOverride = timetableBlock.GeolocationOverride,
            Setting = new SetZoneTemperatureSettingRequest
            {
                DeviceType = timetableBlock.Setting?.DeviceType,
                Power = timetableBlock.Setting?.Power,
                Temperature = timetableBlock.Setting?.Temperature == null
                    ? null
                    : new SetZoneTemperatureValueRequest
                    {
                        Celsius = timetableBlock.Setting.Temperature.Celsius,
                        Fahrenheit = timetableBlock.Setting.Temperature.Fahrenheit
                    },
                Mode = timetableBlock.Setting?.Mode,
                IsBoost = timetableBlock.Setting?.IsBoost
            }
        };
    }
}