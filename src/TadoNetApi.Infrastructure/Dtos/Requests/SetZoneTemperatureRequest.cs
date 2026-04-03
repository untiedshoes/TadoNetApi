using System.Text.Json.Serialization;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Converters;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request DTO to set a zone overlay state.
/// </summary>
public class SetZoneTemperatureRequest
{
    [JsonPropertyName("setting")]
    public SetZoneTemperatureSettingRequest Setting { get; set; } = new();

    [JsonPropertyName("termination")]
    public SetZoneTemperatureTerminationRequest Termination { get; set; } = new();
}

public class SetZoneTemperatureSettingRequest
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(DeviceTypeConverter))]
    public DeviceTypes? DeviceType { get; set; }

    [JsonPropertyName("power")]
    [JsonConverter(typeof(PowerStatesConverter))]
    public PowerStates? Power { get; set; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SetZoneTemperatureValueRequest? Temperature { get; set; }
}

public class SetZoneTemperatureValueRequest
{
    [JsonPropertyName("celsius")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Celsius { get; set; }

    [JsonPropertyName("fahrenheit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Fahrenheit { get; set; }
}

public class SetZoneTemperatureTerminationRequest
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(DurationModeConverter))]
    public DurationModes? CurrentType { get; set; }

    [JsonPropertyName("durationInSeconds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DurationInSeconds { get; set; }
}