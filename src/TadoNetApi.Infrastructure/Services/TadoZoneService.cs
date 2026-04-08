using System.Net;
using System.Text.Json;
using System.Globalization;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Validation;

namespace TadoNetApi.Infrastructure.Services
{
    /// <summary>
    /// Tado REST API implementation for interacting with zones.
    /// </summary>
    public class TadoZoneService : IZoneService
    {
        private readonly ITadoHttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of <see cref="TadoZoneService"/>.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to communicate with the Tado API.</param>
        public TadoZoneService(ITadoHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Data Retrieval

        /// <summary>
        /// Returns a list of zones in the specified home.
        /// </summary>
        /// <param name="homeId">The ID of the home whose zones should be retrieved.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A read-only list of zones.</returns>
        public async Task<IReadOnlyList<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));

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
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested zone.</returns>
        public async Task<Zone> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

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
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The current zone state.</returns>
        public async Task<State> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

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
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The current zone summary, or <see langword="null"/> when no overlay is active.</returns>
        public async Task<ZoneSummary?> GetZoneSummaryAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var response = await _httpClient.GetAsync<TadoZoneSummaryResponse>(
                    $"homes/{homeId}/zones/{zoneId}/overlay",
                    cancellationToken);

                return response == null ? null : ZoneSummaryMapper.ToDomain(response);
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
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A read-only list of zone capabilities.</returns>
        public async Task<IReadOnlyList<Capability>> GetZoneCapabilitiesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

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
        /// Returns the zone control details including heating circuit and grouped devices.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The zone control details.</returns>
        public async Task<ZoneControl> GetZoneControlAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var response = await _httpClient.GetAsync<TadoZoneControlResponse>(
                    $"homes/{homeId}/zones/{zoneId}/control",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound, $"Zone control for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone control: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the default overlay configuration for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The default overlay configuration.</returns>
        public async Task<DefaultZoneOverlay> GetDefaultZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var response = await _httpClient.GetAsync<TadoDefaultZoneOverlayResponse>(
                    $"homes/{homeId}/zones/{zoneId}/defaultOverlay",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound, $"Default zone overlay for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve default zone overlay: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns early start settings for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The early start configuration.</returns>
        public async Task<EarlyStart> GetEarlyStartAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

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
        /// Returns the away-configuration settings for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The away configuration for the zone.</returns>
        public async Task<AwayConfiguration> GetAwayConfigurationAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var response = await _httpClient.GetAsync<TadoAwayConfigurationResponse>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/awayConfiguration",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound,
                        $"Away configuration for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve away configuration: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the active timetable type for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The active timetable type.</returns>
        public async Task<TimetableType> GetActiveTimetableTypeAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var response = await _httpClient.GetAsync<TadoTimetableTypeResponse>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/activeTimetable",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound,
                        $"Active timetable type for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve active timetable type: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the timetable types supported by a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The available timetable types.</returns>
        public async Task<IReadOnlyList<TimetableType>> GetZoneTimetablesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var response = await _httpClient.GetAsync<List<TadoTimetableTypeResponse>>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/timetables",
                    cancellationToken) ?? new List<TadoTimetableTypeResponse>();

                return response.Select(timetable => timetable.ToDomain()).ToList();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone timetables: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a specific timetable type definition for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested timetable type.</returns>
        public async Task<TimetableType> GetZoneTimetableAsync(int homeId, int zoneId, int timetableTypeId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));
            Guard.PositiveId(timetableTypeId, nameof(timetableTypeId));

            try
            {
                var response = await _httpClient.GetAsync<TadoTimetableTypeResponse>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound,
                        $"Timetable type {timetableTypeId} for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone timetable: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns all timetable blocks for a specific timetable type.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The timetable blocks for the specified timetable type.</returns>
        public async Task<IReadOnlyList<TimetableBlock>> GetZoneTimetableBlocksAsync(int homeId, int zoneId, int timetableTypeId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));
            Guard.PositiveId(timetableTypeId, nameof(timetableTypeId));

            try
            {
                var response = await _httpClient.GetAsync<List<TadoTimetableBlockResponse>>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}/blocks",
                    cancellationToken) ?? new List<TadoTimetableBlockResponse>();

                return response.Select(block => block.ToDomain()).ToList();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve timetable blocks: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns timetable blocks for a specific day type.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to inspect.</param>
        /// <param name="dayType">The day type to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The timetable blocks for the specified day type.</returns>
        public async Task<IReadOnlyList<TimetableBlock>> GetTimetableBlocksByDayTypeAsync(int homeId, int zoneId, int timetableTypeId, string dayType, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));
            Guard.PositiveId(timetableTypeId, nameof(timetableTypeId));

            Guard.NotNullOrWhiteSpace(dayType, nameof(dayType));

            try
            {
                var response = await _httpClient.GetAsync<List<TadoTimetableBlockResponse>>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}/blocks/{Uri.EscapeDataString(dayType)}",
                    cancellationToken) ?? new List<TadoTimetableBlockResponse>();

                return response.Select(block => block.ToDomain()).ToList();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve timetable blocks for day type: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the day-report payload for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="date">An optional report date. When omitted, the current date is used.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested day report.</returns>
        public async Task<ZoneDayReport> GetZoneDayReportAsync(int homeId, int zoneId, DateOnly? date = null, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var path = $"homes/{homeId}/zones/{zoneId}/dayReport";
                if (date.HasValue)
                    path += $"?date={date.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";

                var response = await _httpClient.GetAsync<TadoZoneDayReportResponse>(
                    path,
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound,
                        $"Day report for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone day report: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns temperature offset for a zone using the first device serial in zone.Devices.
        /// </summary>
        /// <param name="zone">The zone whose measuring device offset should be resolved.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resolved temperature offset.</returns>
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

        #endregion

        #region Send Commands

        /// <summary>
        /// Enables or disables the early start mode for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="enabled">True to enable early start, false to disable it.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns><see langword="true"/> when the command succeeds.</returns>
        public async Task<bool> SetEarlyStartAsync(int homeId, int zoneId, bool enabled, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await _httpClient.SendAsync(
                $"homes/{homeId}/zones/{zoneId}/earlyStart",
                HttpMethod.Put,
                cancellationToken,
                HttpStatusCode.OK,
                new { enabled });
        }

        /// <summary>
        /// Updates the open window detection settings for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="settings">The open window detection settings to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public async Task SetOpenWindowDetectionAsync(int homeId, int zoneId, OpenWindowDetection settings, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            ArgumentNullException.ThrowIfNull(settings);

            if (settings.Enabled is null)
                throw new ArgumentException("Open window detection enabled state must be provided.", nameof(settings));

            if (settings.TimeoutInSeconds is null)
                throw new ArgumentException("Open window detection timeout must be provided.", nameof(settings));

            if (settings.TimeoutInSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(settings), "Open window detection timeout must be zero or greater.");

            var request = SetOpenWindowDetectionRequest.FromDomain(zoneId, settings);

            await _httpClient.SendAsync(
                $"homes/{homeId}/zones/{zoneId}/openWindowDetection",
                HttpMethod.Put,
                cancellationToken,
                HttpStatusCode.NoContent,
                request);
        }

            /// <summary>
            /// Activates the open window state for a zone.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneId">The ID of the zone.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            public async Task ActivateOpenWindowAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));
                Guard.PositiveId(zoneId, nameof(zoneId));

                await _httpClient.SendAsync(
                    $"homes/{homeId}/zones/{zoneId}/state/openWindow/activate",
                    HttpMethod.Post,
                    cancellationToken,
                    HttpStatusCode.NoContent,
                    null);
            }

            /// <summary>
            /// Resets the open window state for a zone.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneId">The ID of the zone.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            public async Task ResetOpenWindowAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));
                Guard.PositiveId(zoneId, nameof(zoneId));

                await _httpClient.SendAsync(
                    $"homes/{homeId}/zones/{zoneId}/state/openWindow",
                    HttpMethod.Delete,
                    cancellationToken,
                    HttpStatusCode.NoContent,
                    null);
            }

            /// <summary>
            /// Updates the writable details of a zone.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneId">The ID of the zone.</param>
            /// <param name="zoneDetails">The writable zone details payload.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            /// <returns>The updated zone definition.</returns>
            public async Task<Zone> SetZoneDetailsAsync(int homeId, int zoneId, Zone zoneDetails, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));
                Guard.PositiveId(zoneId, nameof(zoneId));

                ArgumentNullException.ThrowIfNull(zoneDetails);

                Guard.NotNullOrWhiteSpace(zoneDetails.Name, nameof(zoneDetails));

                var request = SetZoneDetailsRequest.FromDomain(zoneDetails);

                var response = await _httpClient.PutAsync<SetZoneDetailsRequest, TadoZoneResponse>(
                    $"homes/{homeId}/zones/{zoneId}/details",
                    request,
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound, $"Zone {zoneId} not found.");

                return response.ToDomain();
            }

            /// <summary>
            /// Updates the default overlay configuration for a zone.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneId">The ID of the zone.</param>
            /// <param name="defaultOverlay">The default overlay configuration to apply.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            /// <returns>The updated default overlay configuration.</returns>
            public async Task<DefaultZoneOverlay> SetDefaultZoneOverlayAsync(int homeId, int zoneId, DefaultZoneOverlay defaultOverlay, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));
                Guard.PositiveId(zoneId, nameof(zoneId));

                ArgumentNullException.ThrowIfNull(defaultOverlay);

                if (defaultOverlay.TerminationCondition == null)
                    throw new ArgumentException("Default overlay termination condition must be provided.", nameof(defaultOverlay));

                Guard.NotNullOrWhiteSpace(defaultOverlay.TerminationCondition.Type, nameof(defaultOverlay));

                if (string.Equals(defaultOverlay.TerminationCondition.Type, DurationModes.Timer.ToString(), StringComparison.OrdinalIgnoreCase)
                    && (!defaultOverlay.TerminationCondition.DurationInSeconds.HasValue || defaultOverlay.TerminationCondition.DurationInSeconds.Value <= 0))
                {
                    throw new ArgumentException("Default overlay timer duration must be greater than zero when the termination type is TIMER.", nameof(defaultOverlay));
                }

                var request = SetDefaultZoneOverlayRequest.FromDomain(defaultOverlay);

                var response = await _httpClient.PutAsync<SetDefaultZoneOverlayRequest, TadoDefaultZoneOverlayResponse>(
                    $"homes/{homeId}/zones/{zoneId}/defaultOverlay",
                    request,
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound, $"Default overlay for zone {zoneId} not found.");

                return response.ToDomain();
            }

            /// <summary>
            /// Applies manual overlays to multiple zones in a single home-level command.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneOverlays">The zone IDs and overlay payloads to apply.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            public async Task SetZoneOverlaysAsync(int homeId, IReadOnlyDictionary<int, Overlay> zoneOverlays, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));

                if (zoneOverlays == null)
                    throw new ArgumentNullException(nameof(zoneOverlays));

                if (zoneOverlays.Count == 0)
                    throw new ArgumentException("At least one zone overlay is required.", nameof(zoneOverlays));

                if (zoneOverlays.Keys.Any(zoneId => zoneId <= 0))
                    throw new ArgumentOutOfRangeException(nameof(zoneOverlays), "Zone IDs must all be positive integers.");

                foreach (var zoneOverlay in zoneOverlays.Values)
                {
                    ValidateOverlay(zoneOverlay, nameof(zoneOverlays));
                }

                var request = SetZoneOverlaysRequest.FromDomain(zoneOverlays);

                await _httpClient.SendAsync(
                    $"homes/{homeId}/overlay",
                    HttpMethod.Post,
                    cancellationToken,
                    HttpStatusCode.NoContent,
                    request);
            }

            /// <summary>
            /// Deletes the currently active manual overlays for multiple zones in a single home-level command.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneIds">The IDs of the zones whose overlays should be deleted.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            public async Task DeleteZoneOverlaysAsync(int homeId, IReadOnlyList<int> zoneIds, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));

                if (zoneIds == null)
                    throw new ArgumentNullException(nameof(zoneIds));

                if (zoneIds.Count == 0)
                    throw new ArgumentException("At least one zone ID is required.", nameof(zoneIds));

                if (zoneIds.Any(zoneId => zoneId <= 0))
                    throw new ArgumentOutOfRangeException(nameof(zoneIds), "Zone IDs must all be positive integers.");

                var query = string.Join("&", zoneIds.Select(zoneId => $"rooms={zoneId.ToString(CultureInfo.InvariantCulture)}"));

                await _httpClient.SendAsync(
                    $"homes/{homeId}/overlay?{query}",
                    HttpMethod.Delete,
                    cancellationToken,
                    HttpStatusCode.NoContent,
                    null);
            }

            /// <summary>
            /// Updates the settings used for a zone while the home is in AWAY mode.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneId">The ID of the zone.</param>
            /// <param name="awayConfiguration">The away configuration to apply.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            public async Task SetAwayConfigurationAsync(int homeId, int zoneId, AwayConfiguration awayConfiguration, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));
                Guard.PositiveId(zoneId, nameof(zoneId));

                ArgumentNullException.ThrowIfNull(awayConfiguration);

                Guard.NotNullOrWhiteSpace(awayConfiguration.Type, nameof(awayConfiguration));

                if (awayConfiguration.Setting == null)
                    throw new ArgumentException("Away configuration setting must be provided.", nameof(awayConfiguration));

                if (awayConfiguration.Setting.DeviceType == null)
                    throw new ArgumentException("Away configuration setting type must be provided.", nameof(awayConfiguration));

                if (awayConfiguration.Setting.Power == null)
                    throw new ArgumentException("Away configuration power state must be provided.", nameof(awayConfiguration));

                var request = SetAwayConfigurationRequest.FromDomain(awayConfiguration);

                await _httpClient.SendAsync(
                    $"homes/{homeId}/zones/{zoneId}/schedule/awayConfiguration",
                    HttpMethod.Put,
                    cancellationToken,
                    HttpStatusCode.NoContent,
                    request);
            }

            /// <summary>
            /// Updates the active timetable type for a zone.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneId">The ID of the zone.</param>
            /// <param name="timetableType">The timetable type to activate.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            /// <returns>The active timetable type returned by the API.</returns>
            public async Task<TimetableType> SetActiveTimetableTypeAsync(int homeId, int zoneId, TimetableType timetableType, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));
                Guard.PositiveId(zoneId, nameof(zoneId));

                ArgumentNullException.ThrowIfNull(timetableType);

                if (!timetableType.Id.HasValue || timetableType.Id.Value <= 0)
                    throw new ArgumentException("Timetable type ID must be provided and greater than zero.", nameof(timetableType));

                var request = SetActiveTimetableTypeRequest.FromDomain(timetableType);

                var response = await _httpClient.PutAsync<SetActiveTimetableTypeRequest, TadoTimetableTypeResponse>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/activeTimetable",
                    request,
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound,
                        $"Active timetable type for zone {zoneId} not found.");

                return response.ToDomain();
            }

            /// <summary>
            /// Updates timetable blocks for a specific day type.
            /// </summary>
            /// <param name="homeId">The ID of the home.</param>
            /// <param name="zoneId">The ID of the zone.</param>
            /// <param name="timetableTypeId">The timetable type ID to update.</param>
            /// <param name="dayType">The day type to update.</param>
            /// <param name="blocks">The timetable blocks to apply.</param>
            /// <param name="cancellationToken">The cancellation token to observe.</param>
            /// <returns>The persisted timetable blocks returned by the API.</returns>
            public async Task<IReadOnlyList<TimetableBlock>> SetTimetableBlocksForDayTypeAsync(int homeId, int zoneId, int timetableTypeId, string dayType, IReadOnlyList<TimetableBlock> blocks, CancellationToken cancellationToken = default)
            {
                Guard.PositiveId(homeId, nameof(homeId));
                Guard.PositiveId(zoneId, nameof(zoneId));
                Guard.PositiveId(timetableTypeId, nameof(timetableTypeId));

                Guard.NotNullOrWhiteSpace(dayType, nameof(dayType));

                if (blocks == null)
                    throw new ArgumentNullException(nameof(blocks));

                foreach (var block in blocks)
                {
                    ValidateTimetableBlock(block, nameof(blocks));
                }

                var request = blocks.Select(SetTimetableBlockRequest.FromDomain).ToList();

                var response = await _httpClient.PutAsync<List<SetTimetableBlockRequest>, List<TadoTimetableBlockResponse>>(
                    $"homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}/blocks/{Uri.EscapeDataString(dayType)}",
                    request,
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(HttpStatusCode.NotFound,
                        $"Timetable blocks for zone {zoneId}, timetable type {timetableTypeId}, and day type {dayType} not found.");

                return response.Select(block => block.ToDomain()).ToList();
            }

        /// <summary>
        /// Creates a new zone and moves the specified devices into it.
        /// </summary>
        /// <param name="homeId">The ID of the home in which the zone should be created.</param>
        /// <param name="zoneType">The zone type, such as HEATING or HOT_WATER.</param>
        /// <param name="deviceSerialNumbers">The device serial numbers to move into the new zone.</param>
        /// <param name="force">Optional flag indicating whether the previous zone should be forcibly removed when supported.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public async Task CreateZoneAsync(int homeId, string zoneType, IReadOnlyList<string> deviceSerialNumbers, bool? force = null, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));

            Guard.NotNullOrWhiteSpace(zoneType, nameof(zoneType));

            if (deviceSerialNumbers == null)
                throw new ArgumentNullException(nameof(deviceSerialNumbers));

            if (deviceSerialNumbers.Count == 0)
                throw new ArgumentException("At least one device serial number is required.", nameof(deviceSerialNumbers));

            foreach (var deviceSerialNumber in deviceSerialNumbers)
            {
                Guard.NotNullOrWhiteSpace(deviceSerialNumber, nameof(deviceSerialNumbers));
            }

            var request = new CreateZoneRequest
            {
                ZoneType = zoneType,
                Devices = deviceSerialNumbers
                    .Select(serialNo => new CreateZoneDeviceRequest { SerialNo = serialNo })
                    .ToArray()
            };

            var path = $"homes/{homeId}/zones";
            if (force.HasValue)
                path += $"?force={force.Value.ToString().ToLowerInvariant()}";

            await _httpClient.SendAsync(
                path,
                HttpMethod.Post,
                cancellationToken,
                HttpStatusCode.Created,
                request);
        }

        /// <summary>
        /// Deletes the currently active manual overlay for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone whose overlay should be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns><see langword="true"/> when the command succeeds.</returns>
        public async Task<bool> DeleteZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await _httpClient.SendAsync(
                $"homes/{homeId}/zones/{zoneId}/overlay",
                HttpMethod.Delete,
                cancellationToken,
                HttpStatusCode.NoContent,
                null);
        }

        /// <summary>
        /// Assigns the zone to a specific heating circuit or removes the assignment.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="circuitNumber">The heating circuit number to assign, or null to remove the assignment.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone control details.</returns>
        public async Task<ZoneControl> SetHeatingCircuitAsync(int homeId, int zoneId, int? circuitNumber, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            if (circuitNumber.HasValue && circuitNumber.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(circuitNumber), "Circuit number must be a positive integer when provided.");

            var request = circuitNumber.HasValue
                ? new SetHeatingCircuitRequest { CircuitNumber = circuitNumber.Value }
                : null;

            var response = await _httpClient.PutAsync<SetHeatingCircuitRequest?, TadoZoneControlResponse>(
                $"homes/{homeId}/zones/{zoneId}/control/heatingCircuit",
                request,
                cancellationToken);

            if (response == null)
                throw new TadoApiException(HttpStatusCode.NotFound, $"Zone control for zone {zoneId} not found.");

            return response.ToDomain();
        }

        /// <summary>
        /// Sets the heating temperature in Celsius for a zone, keeping it until the next manual change.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone summary, or null if the response could not be deserialized.</returns>
        public Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
            => SetHeatingTemperatureCelsiusAsync(homeId, zoneId, temperature, DurationModes.UntilNextManualChange, null, cancellationToken);

        /// <summary>
        /// Sets the heating temperature in Fahrenheit for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="temperature">The target temperature in Fahrenheit.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone summary, or null if the response could not be deserialized.</returns>
        public async Task<ZoneSummary?> SetHeatingTemperatureFahrenheitAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await SetTemperatureAsync(homeId, zoneId, null, temperature, DeviceTypes.Heating, DurationModes.UntilNextManualChange, null, cancellationToken);
        }

        /// <summary>
        /// Sets the hot water temperature in Celsius for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone summary, or null if the response could not be deserialized.</returns>
        public async Task<ZoneSummary?> SetHotWaterTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await SetTemperatureAsync(homeId, zoneId, temperature, null, DeviceTypes.HotWater, DurationModes.UntilNextManualChange, null, cancellationToken);
        }

        /// <summary>
        /// Sets the hot water temperature in Fahrenheit for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="temperature">The target temperature in Fahrenheit.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone summary, or null if the response could not be deserialized.</returns>
        public async Task<ZoneSummary?> SetHotWaterTemperatureFahrenheitAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await SetTemperatureAsync(homeId, zoneId, null, temperature, DeviceTypes.HotWater, DurationModes.UntilNextManualChange, null, cancellationToken);
        }

        /// <summary>
        /// Switches heating off for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone summary, or null if the response could not be deserialized.</returns>
        public async Task<ZoneSummary?> SwitchHeatingOffAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await SetTemperatureAsync(homeId, zoneId, null, null, DeviceTypes.Heating, DurationModes.UntilNextManualChange, null, cancellationToken);
        }

        /// <summary>
        /// Switches hot water off for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone summary, or null if the response could not be deserialized.</returns>
        public async Task<ZoneSummary?> SwitchHotWaterOffAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await SetTemperatureAsync(homeId, zoneId, null, null, DeviceTypes.HotWater, DurationModes.UntilNextManualChange, null, cancellationToken);
        }

        /// <summary>
        /// Sets the heating temperature in Celsius for a zone for the specified duration.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="durationMode">How long the setting should remain active.</param>
        /// <param name="timer">Required when <paramref name="durationMode"/> is <see cref="DurationModes.Timer"/>.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone summary, or null if the response could not be deserialized.</returns>
        public async Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, DurationModes durationMode, TimeSpan? timer = null, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            return await SetTemperatureAsync(homeId, zoneId, temperature, null, DeviceTypes.Heating, durationMode, timer, cancellationToken);
        }

        private static void ValidateOverlay(Overlay overlay, string paramName)
        {
            ArgumentNullException.ThrowIfNull(overlay);

            if (overlay.Setting == null)
                throw new ArgumentException("Zone overlay setting must be provided.", paramName);

            if (overlay.Setting.DeviceType == null)
                throw new ArgumentException("Zone overlay setting type must be provided.", paramName);

            if (overlay.Setting.Power == null)
                throw new ArgumentException("Zone overlay power state must be provided.", paramName);

            if (overlay.Termination == null)
                throw new ArgumentException("Zone overlay termination must be provided.", paramName);

            Guard.NotNullOrWhiteSpace(overlay.Termination.Type, paramName);

            if (IsTimerTermination(overlay.Termination.Type)
                && (!overlay.Termination.DurationInSeconds.HasValue || overlay.Termination.DurationInSeconds.Value <= 0))
            {
                throw new ArgumentException("Zone overlay timer duration must be greater than zero when the termination type is TIMER.", paramName);
            }
        }

        private static bool IsTimerTermination(string? type)
            => string.Equals(type, nameof(DurationModes.Timer), StringComparison.OrdinalIgnoreCase)
                || string.Equals(type, "TIMER", StringComparison.OrdinalIgnoreCase);

        private static void ValidateTimetableBlock(TimetableBlock timetableBlock, string paramName)
        {
            ArgumentNullException.ThrowIfNull(timetableBlock);

            Guard.NotNullOrWhiteSpace(timetableBlock.DayType, paramName);

            Guard.NotNullOrWhiteSpace(timetableBlock.Start, paramName);

            Guard.NotNullOrWhiteSpace(timetableBlock.End, paramName);

            if (timetableBlock.Setting == null)
                throw new ArgumentException("Timetable block setting must be provided.", paramName);

            if (timetableBlock.Setting.DeviceType == null)
                throw new ArgumentException("Timetable block setting type must be provided.", paramName);

            if (timetableBlock.Setting.Power == null)
                throw new ArgumentException("Timetable block power state must be provided.", paramName);
        }

        private async Task<ZoneSummary?> SetTemperatureAsync(int homeId, int zoneId, double? temperatureCelsius, double? temperatureFahrenheit, DeviceTypes deviceType, DurationModes durationMode, TimeSpan? timer, CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(deviceType))
                throw new ArgumentOutOfRangeException(nameof(deviceType));

            if (!Enum.IsDefined(durationMode))
                throw new ArgumentOutOfRangeException(nameof(durationMode));

            if (durationMode == DurationModes.Timer && timer == null)
                durationMode = DurationModes.UntilNextManualChange;

            var request = new SetZoneTemperatureRequest
            {
                Setting = new SetZoneTemperatureSettingRequest
                {
                    DeviceType = deviceType,
                    Power = !temperatureCelsius.HasValue && !temperatureFahrenheit.HasValue
                        ? PowerStates.Off
                        : PowerStates.On
                },
                Termination = new SetZoneTemperatureTerminationRequest
                {
                    CurrentType = durationMode
                }
            };

            if (request.Setting.Power == PowerStates.On)
            {
                request.Setting.Temperature = new SetZoneTemperatureValueRequest
                {
                    Celsius = temperatureCelsius,
                    Fahrenheit = temperatureFahrenheit
                };
            }

            if (durationMode == DurationModes.Timer && timer.HasValue)
                request.Termination.DurationInSeconds = (int)timer.Value.TotalSeconds;

            var response = await _httpClient.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                $"homes/{homeId}/zones/{zoneId}/overlay",
                request,
                cancellationToken);

            return response == null ? null : ZoneSummaryMapper.ToDomain(response);
        }

        #endregion

    }
}