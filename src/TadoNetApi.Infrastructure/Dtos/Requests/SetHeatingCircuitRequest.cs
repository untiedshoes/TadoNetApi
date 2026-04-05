using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for assigning a zone to a heating circuit.
/// </summary>
public class SetHeatingCircuitRequest
{
    /// <summary>
    /// The heating circuit number to assign to the zone.
    /// </summary>
    [JsonPropertyName("circuitNumber")]
    public int? CircuitNumber { get; set; }
}