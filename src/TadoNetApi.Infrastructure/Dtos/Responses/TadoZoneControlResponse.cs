using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the API response for zone control details.
    /// </summary>
    public class TadoZoneControlResponse
    {
        /// <summary>
        /// The zone type, for example HEATING or HOT_WATER.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Indicates whether early start is enabled.
        /// </summary>
        [JsonPropertyName("earlyStartEnabled")]
        public bool? EarlyStartEnabled { get; set; }

        /// <summary>
        /// The heating circuit number associated with the zone.
        /// </summary>
        [JsonPropertyName("heatingCircuit")]
        public int? HeatingCircuit { get; set; }

        /// <summary>
        /// The devices grouped by duty.
        /// </summary>
        [JsonPropertyName("duties")]
        public TadoZoneControlDutiesResponse? Duties { get; set; }
    }

    /// <summary>
    /// Represents grouped zone devices by duty.
    /// </summary>
    public class TadoZoneControlDutiesResponse
    {
        /// <summary>
        /// The zone type associated with this duty grouping.
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// The single driver device, when present.
        /// </summary>
        [JsonPropertyName("driver")]
        public TadoDeviceResponse? Driver { get; set; }

        /// <summary>
        /// The driver devices for the zone.
        /// </summary>
        [JsonPropertyName("drivers")]
        public TadoDeviceResponse[]? Drivers { get; set; }

        /// <summary>
        /// The single leader device, when present.
        /// </summary>
        [JsonPropertyName("leader")]
        public TadoDeviceResponse? Leader { get; set; }

        /// <summary>
        /// The leader devices for the zone.
        /// </summary>
        [JsonPropertyName("leaders")]
        public TadoDeviceResponse[]? Leaders { get; set; }

        /// <summary>
        /// The single UI device, when present.
        /// </summary>
        [JsonPropertyName("ui")]
        public TadoDeviceResponse? Ui { get; set; }

        /// <summary>
        /// The UI devices for the zone.
        /// </summary>
        [JsonPropertyName("uis")]
        public TadoDeviceResponse[]? Uis { get; set; }
    }
}