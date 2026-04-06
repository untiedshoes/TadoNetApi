using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating flow-temperature optimisation.
/// </summary>
public class SetFlowTemperatureOptimisationRequest
{
    /// <summary>
    /// The maximum flow temperature to apply.
    /// </summary>
    [JsonPropertyName("maxFlowTemperature")]
    public int MaxFlowTemperature { get; set; }
}