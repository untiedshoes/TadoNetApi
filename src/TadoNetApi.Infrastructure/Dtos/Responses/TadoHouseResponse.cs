using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Contains detailed information about a house
    /// </summary>
    public class TadoHouseResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("dateTimeZone")]
        public string DateTimeZone { get; set; } = string.Empty;

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("temperatureUnit")]
        public string TemperatureUnit { get; set; } = string.Empty;

        [JsonPropertyName("installationCompleted")]
        public bool InstallationCompleted { get; set; }

        [JsonPropertyName("partner")]
        public object? Partner { get; set; }

        [JsonPropertyName("simpleSmartScheduleEnabled")]
        public bool SimpleSmartScheduleEnabled { get; set; }

        [JsonPropertyName("awayRadiusInMeters")]
        public double AwayRadiusInMeters { get; set; }

        [JsonPropertyName("license")]
        public string License { get; set; } = string.Empty;

        [JsonPropertyName("christmasModeEnabled")]
        public bool ChristmasModeEnabled { get; set; }

        [JsonPropertyName("incidentDetection")]
        public TadoIncidentDetectionResponse? IncidentDetection { get; set; }

        [JsonPropertyName("contactDetails")]
        public TadoContactDetailsResponse ContactDetails { get; set; } = new();

        [JsonPropertyName("address")]
        public TadoAddressResponse Address { get; set; } = new();

        [JsonPropertyName("geolocation")]
        public TadoGeolocationResponse Geolocation { get; set; } = new();
    }
}