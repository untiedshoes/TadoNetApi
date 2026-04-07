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

            if (string.IsNullOrWhiteSpace(zoneType))
                throw new ArgumentException("Zone type is required.", nameof(zoneType));

            if (deviceSerialNumbers == null)
                throw new ArgumentNullException(nameof(deviceSerialNumbers));

            if (deviceSerialNumbers.Count == 0)
                throw new ArgumentException("At least one device serial number is required.", nameof(deviceSerialNumbers));

            if (deviceSerialNumbers.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Device serial numbers cannot be null, empty, or whitespace.", nameof(deviceSerialNumbers));

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
                HttpStatusCode.OK,
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