using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the heating system response from the API.
/// </summary>
public class TadoHeatingSystemResponse
{
    /// <summary>
    /// Boiler details.
    /// </summary>
    [JsonPropertyName("boiler")]
    public TadoBoilerResponse? Boiler { get; set; }

    /// <summary>
    /// Underfloor heating details.
    /// </summary>
    [JsonPropertyName("underfloorHeating")]
    public TadoUnderfloorHeatingResponse? UnderfloorHeating { get; set; }
}

/// <summary>
/// Represents boiler details from the API.
/// </summary>
public class TadoBoilerResponse
{
    /// <summary>
    /// Indicates whether a boiler is present.
    /// </summary>
    [JsonPropertyName("present")]
    public bool? Present { get; set; }

    /// <summary>
    /// Tado-specific boiler identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    /// <summary>
    /// Indicates whether the boiler was found.
    /// </summary>
    [JsonPropertyName("found")]
    public bool? Found { get; set; }
}

/// <summary>
/// Represents underfloor heating details from the API.
/// </summary>
public class TadoUnderfloorHeatingResponse
{
    /// <summary>
    /// Indicates whether underfloor heating is present.
    /// </summary>
    [JsonPropertyName("present")]
    public bool? Present { get; set; }
}