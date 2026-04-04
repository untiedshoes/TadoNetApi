using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the API response for home flow-temperature optimisation.
/// </summary>
public class TadoFlowTemperatureOptimisationResponse
{
    [JsonPropertyName("hasMultipleBoilerControlDevices")]
    public bool? HasMultipleBoilerControlDevices { get; set; }

    [JsonPropertyName("maxFlowTemperature")]
    public int? MaxFlowTemperature { get; set; }

    [JsonPropertyName("maxFlowTemperatureConstraints")]
    public TadoFlowTemperatureOptimisationConstraintsResponse? MaxFlowTemperatureConstraints { get; set; }

    [JsonPropertyName("autoAdaptation")]
    public TadoFlowTemperatureOptimisationAutoAdaptationResponse? AutoAdaptation { get; set; }

    [JsonPropertyName("openThermDeviceSerialNumber")]
    public string? OpenThermDeviceSerialNumber { get; set; }
}

/// <summary>
/// Represents min/max constraints for flow temperature.
/// </summary>
public class TadoFlowTemperatureOptimisationConstraintsResponse
{
    [JsonPropertyName("min")]
    public int? Min { get; set; }

    [JsonPropertyName("max")]
    public int? Max { get; set; }
}

/// <summary>
/// Represents auto-adaptation details in the flow-temperature optimisation response.
/// </summary>
public class TadoFlowTemperatureOptimisationAutoAdaptationResponse
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonPropertyName("maxFlowTemperature")]
    public int? MaxFlowTemperature { get; set; }
}