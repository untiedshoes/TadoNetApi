using System.Net;
using System.Text.Json;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Infrastructure.Services
{
    /// <summary>
    /// Tado REST API implementation for interacting with zones.
    /// </summary>
    public class TadoZoneService : IZoneService
    {
        private readonly ITadoHttpClient _httpClient;

        public TadoZoneService(ITadoHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Returns a list of zones in the specified home.
        /// </summary>
        public async Task<IReadOnlyList<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync<List<TadoZoneResponse>>(
                    $"homes/{homeId}/zones",
                    cancellationToken) ?? new List<TadoZoneResponse>();

                return response.Select(z => z.ToDomain()).ToList();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zones: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns detailed information about a specific zone.
        /// </summary>
        public async Task<Zone> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync<TadoZoneResponse>(
                    $"homes/{homeId}/zones/{zoneId}",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound, $"Zone {zoneId} not found.");

                return ZoneMapper.ToDomain(response);
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the current state of a zone (current temperature, humidity, etc.)
        /// </summary>
        public async Task<State> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync<TadoStateResponse>(
                    $"homes/{homeId}/zones/{zoneId}/state",
                    cancellationToken) ?? new TadoStateResponse();

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound, $"Zone state for zone {zoneId} not found.");

                return StateMapper.ToDomain(response);
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone state: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the summarized state of a zone (target temperature, overlay, etc.)
        /// </summary>
        public async Task<ZoneSummary?> GetZoneSummaryAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync<TadoZoneSummaryResponse>(
                    $"homes/{homeId}/zones/{zoneId}/overlay",
                    cancellationToken);

                return ZoneSummaryMapper.ToDomain(response);
            }
            catch (TadoApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // No overlay active for this zone
                return null;
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone summary: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the capabilities of a zone.
        /// </summary>
        public async Task<IReadOnlyList<Capability>> GetZoneCapabilitiesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            try
            {
                var rawResponse = await _httpClient.GetAsync<JsonElement>(
                    $"homes/{homeId}/zones/{zoneId}/capabilities",
                    cancellationToken);

                // The endpoint can return either null/undefined, an array, or a single object.
                if (rawResponse.ValueKind == JsonValueKind.Undefined || rawResponse.ValueKind == JsonValueKind.Null)
                    return new List<Capability>();

                var capabilities = new List<Capability>();

                // Standard payload shape: array of capabilities.
                if (rawResponse.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in rawResponse.EnumerateArray())
                    {
                        var dto = JsonSerializer.Deserialize<TadoCapabilityResponse>(item.GetRawText());
                        if (dto != null)
                            capabilities.Add(dto.ToDomain());
                    }

                    return capabilities;
                }

                // Alternate payload shape: single capability object.
                if (rawResponse.ValueKind == JsonValueKind.Object)
                {
                    var dto = JsonSerializer.Deserialize<TadoCapabilityResponse>(rawResponse.GetRawText());
                    if (dto != null)
                        capabilities.Add(dto.ToDomain());

                    return capabilities;
                }

                throw new TadoApiException(HttpStatusCode.UnprocessableEntity,
                    "Unexpected zone capabilities payload format.");
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone capabilities: {ex.Message}");
            }
            catch (JsonException ex)
            {
                throw new TadoApiException(HttpStatusCode.UnprocessableEntity,
                    $"Failed to parse zone capabilities: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns early start settings for a zone.
        /// </summary>
        public async Task<EarlyStart> GetEarlyStartAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync<TadoEarlyStartResponse>(
                    $"homes/{homeId}/zones/{zoneId}/earlyStart",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound, $"Early start settings for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve early start settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns temperature offset for a zone using the first device serial in zone.Devices.
        /// </summary>
        public async Task<Temperature> GetZoneTemperatureOffsetAsync(Zone zone, CancellationToken cancellationToken = default)
        {
            if (zone == null) throw new ArgumentNullException(nameof(zone));

            var deviceSerial = zone.Devices?.FirstOrDefault()?.ShortSerialNo;
            if (string.IsNullOrEmpty(deviceSerial))
                throw new ArgumentException("Zone does not contain any device with a valid short serial.", nameof(zone));

            try
            {
                var response = await _httpClient.GetAsync<TadoTemperatureResponse>(
                    $"devices/{deviceSerial}/temperatureOffset",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound,
                        $"Temperature offset for device {deviceSerial} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone temperature offset: {ex.Message}");
            }
        }
    }
}