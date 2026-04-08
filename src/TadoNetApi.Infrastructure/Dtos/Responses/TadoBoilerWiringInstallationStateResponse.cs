using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the boiler wiring installation state returned by bridge-scoped diagnostics.
    /// </summary>
    public class TadoBoilerWiringInstallationStateResponse
    {
        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("deviceWiredToBoiler")]
        public TadoBoilerWiredDeviceResponse? DeviceWiredToBoiler { get; set; }

        [JsonPropertyName("bridgeConnected")]
        public bool? BridgeConnected { get; set; }

        [JsonPropertyName("hotWaterZonePresent")]
        public bool? HotWaterZonePresent { get; set; }

        [JsonPropertyName("boiler")]
        public TadoBoilerWiringBoilerResponse? Boiler { get; set; }
    }

    public class TadoBoilerWiredDeviceResponse
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("serialNo")]
        public string? SerialNo { get; set; }

        [JsonPropertyName("thermInterfaceType")]
        public string? ThermInterfaceType { get; set; }

        [JsonPropertyName("connected")]
        public bool? Connected { get; set; }

        [JsonPropertyName("lastRequestTimestamp")]
        public DateTime? LastRequestTimestamp { get; set; }
    }

    public class TadoBoilerWiringBoilerResponse
    {
        [JsonPropertyName("outputTemperature")]
        public TadoBoilerOutputTemperatureResponse? OutputTemperature { get; set; }
    }

    public class TadoBoilerOutputTemperatureResponse
    {
        [JsonPropertyName("celsius")]
        public double? Celsius { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}