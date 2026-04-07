using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating the away configuration of a zone.
/// </summary>
public sealed class SetAwayConfigurationRequest
{
    /// <summary>
    /// Gets or sets the zone type, such as HEATING or HOT_WATER.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the away configuration should auto-adjust.
    /// </summary>
    [JsonPropertyName("autoAdjust")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AutoAdjust { get; set; }

    /// <summary>
    /// Gets or sets the optional comfort level.
    /// </summary>
    [JsonPropertyName("comfortLevel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ComfortLevel { get; set; }

    /// <summary>
    /// Gets or sets the setting applied while the home is away.
    /// </summary>
    [JsonPropertyName("setting")]
    public SetZoneTemperatureSettingRequest Setting { get; set; } = new();

    /// <summary>
    /// Maps a domain <see cref="AwayConfiguration"/> into the API request shape.
    /// </summary>
    public static SetAwayConfigurationRequest FromDomain(AwayConfiguration awayConfiguration)
    {
        ArgumentNullException.ThrowIfNull(awayConfiguration);

        return new SetAwayConfigurationRequest
        {
            Type = awayConfiguration.Type ?? string.Empty,
            AutoAdjust = awayConfiguration.AutoAdjust,
            ComfortLevel = awayConfiguration.ComfortLevel,
            Setting = new SetZoneTemperatureSettingRequest
            {
                DeviceType = awayConfiguration.Setting?.DeviceType,
                Power = awayConfiguration.Setting?.Power,
                Temperature = awayConfiguration.Setting?.Temperature == null
                    ? null
                    : new SetZoneTemperatureValueRequest
                    {
                        Celsius = awayConfiguration.Setting.Temperature.Celsius,
                        Fahrenheit = awayConfiguration.Setting.Temperature.Fahrenheit
                    }
            }
        };
    }
}