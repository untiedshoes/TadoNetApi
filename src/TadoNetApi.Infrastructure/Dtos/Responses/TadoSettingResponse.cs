using System.Text.Json.Serialization;
using TadoNetApi.Domain.Enums;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// The current state of a Tado device
    /// </summary>
    public partial class TadoSettingResponse
    {
        /// <summary>
        /// Type of Tado device
        /// </summary>
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Converters.DeviceTypeConverter))] // Ensure this is a System.Text.Json converter
        public DeviceTypes? DeviceType { get; set; }

        /// <summary>
        /// The power state of the Tado device
        /// </summary>
        [JsonPropertyName("power")]
        [JsonConverter(typeof(Converters.PowerStatesConverter))] // Ensure this is a System.Text.Json converter
        public PowerStates? Power { get; set; }

        /// <summary>
        /// The temperature the Tado device is set to change the zone to
        /// </summary>
        [JsonPropertyName("temperature")]
        public TadoTemperatureResponse? Temperature { get; set; }

        /// <summary>
        /// The active mode when applicable to the zone type.
        /// </summary>
        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        /// <summary>
        /// Indicates whether boost mode is enabled when applicable.
        /// </summary>
        [JsonPropertyName("isBoost")]
        public bool? IsBoost { get; set; }
    }
}