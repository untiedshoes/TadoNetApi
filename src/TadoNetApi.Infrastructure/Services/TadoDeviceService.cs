using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Infrastructure.Validation;

namespace TadoNetApi.Infrastructure.Services
{
    /// <summary>
    /// Provides operations to fetch Tado devices at the home level.
    /// </summary>
    public class TadoDeviceService : IDeviceService
    {
        private readonly ITadoHttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of <see cref="TadoDeviceService"/>.
        /// </summary>
        /// <param name="httpClient">The HTTP client used to communicate with the Tado API.</param>
        public TadoDeviceService(ITadoHttpClient httpClient) =>
            _httpClient = httpClient;

        #region Data Retrieval

        /// <summary>
        /// Retrieves all devices for the specified home.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A read-only list of <see cref="Device"/>.</returns>
        /// <exception cref="TadoApiException">Thrown if the API request fails.</exception>
        public async Task<IReadOnlyList<Device>> GetDevicesAsync(int homeId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));

            try
            {
                var response = await _httpClient.GetAsync<List<TadoDeviceResponse>>(
                    $"homes/{homeId}/devices",
                    cancellationToken) ?? new List<TadoDeviceResponse>();

                return response.ToDomainList();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve devices: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all devices for the specified home including zone association where available.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A read-only list of <see cref="DeviceListEntry"/>.</returns>
        /// <exception cref="TadoApiException">Thrown if the API request fails.</exception>
        public async Task<IReadOnlyList<DeviceListEntry>> GetDeviceListAsync(int homeId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));

            try
            {
                var response = await _httpClient.GetAsync<TadoDeviceListResponse>(
                    $"homes/{homeId}/deviceList",
                    cancellationToken);

                return response?.Entries?.ToDomainList() ?? new List<DeviceListEntry>();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve device list: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a single device by home and device ID.
        /// </summary>
        /// <param name="homeId">The home ID.</param>
        /// <param name="deviceId">The device ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="Device"/>.</returns>
        /// <exception cref="TadoApiException">Thrown if the device is not found or request fails.</exception>
        public async Task<Device> GetDeviceAsync(int homeId, int deviceId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(deviceId, nameof(deviceId));

            try
            {
                var response = await _httpClient.GetAsync<TadoDeviceResponse>(
                    $"homes/{homeId}/devices/{deviceId}",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                        $"Device {deviceId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve device: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the temperature offset for a device.
        /// </summary>
        /// <param name="deviceId">The device ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="Temperature"/> offset for the device.</returns>
        /// <exception cref="TadoApiException">Thrown if request fails.</exception>
        public async Task<Temperature> GetZoneTemperatureOffsetAsync(int deviceId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(deviceId, nameof(deviceId));

            try
            {
                var response = await _httpClient.GetAsync<TadoTemperatureResponse>(
                    $"devices/{deviceId}/temperatureOffset",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                        $"Temperature offset for device {deviceId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve temperature offset: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all mobile devices registered to the specified home.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A read-only list of <see cref="Item"/>.</returns>
        /// <exception cref="TadoApiException">Thrown if the API request fails.</exception>
        public async Task<IReadOnlyList<Item>> GetMobileDevicesAsync(int homeId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));

            try
            {
                var response = await _httpClient.GetAsync<List<TadoMobileItemResponse>>(
                    $"homes/{homeId}/mobileDevices",
                    cancellationToken) ?? new List<TadoMobileItemResponse>();

                return response.ToDomainList();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve mobile devices: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the measuring device for the specified zone.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="Device"/> used as the measuring device for the zone.</returns>
        /// <exception cref="TadoApiException">Thrown if the device is not found or the request fails.</exception>
        public async Task<Device> GetZoneMeasuringDeviceAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(zoneId, nameof(zoneId));

            try
            {
                var response = await _httpClient.GetAsync<TadoDeviceResponse>(
                    $"homes/{homeId}/zones/{zoneId}/measuringDevice",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                        $"Measuring device for zone {zoneId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone measuring device: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a specific mobile device for the specified home.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="mobileDeviceId">The ID of the mobile device.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="Item"/> for the mobile device.</returns>
        /// <exception cref="TadoApiException">Thrown if the device is not found or the request fails.</exception>
        public async Task<Item> GetMobileDeviceAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(mobileDeviceId, nameof(mobileDeviceId));

            try
            {
                var response = await _httpClient.GetAsync<TadoMobileItemResponse>(
                    $"homes/{homeId}/mobileDevices/{mobileDeviceId}",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                        $"Mobile device {mobileDeviceId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve mobile device: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the settings for a specific mobile device.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="mobileDeviceId">The ID of the mobile device.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="Settings"/> for the mobile device.</returns>
        /// <exception cref="TadoApiException">Thrown if the device is not found or request fails.</exception>
        public async Task<Settings> GetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default)
        {
            Guard.PositiveId(homeId, nameof(homeId));
            Guard.PositiveId(mobileDeviceId, nameof(mobileDeviceId));

            try
            {
                var response = await _httpClient.GetAsync<TadoMobileSettingsResponse>(
                    $"homes/{homeId}/mobileDevices/{mobileDeviceId}/settings",
                    cancellationToken);

                if (response == null)
                    throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                        $"Settings for mobile device {mobileDeviceId} not found.");

                return response.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve mobile device settings: {ex.Message}");
            }
        }

        #endregion

        #region Send Commands

        /// <summary>
        /// Turns the child lock on or off on the provided Tado device.
        /// </summary>
        /// <param name="device">The Tado device to set the child lock for.</param>
        /// <param name="enableChildLock">True to enable child lock, false to disable it.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>True if the request was successful; otherwise false when the device has no short serial number.</returns>
        public async Task<bool> SetDeviceChildLockAsync(Device device, bool enableChildLock, CancellationToken cancellationToken = default)
        {
            if (device.ShortSerialNo is null) return false;

            return await SetDeviceChildLockAsync(device.ShortSerialNo, enableChildLock, cancellationToken);
        }

        /// <summary>
        /// Turns the child lock on or off on the Tado device with the provided ID.
        /// </summary>
        /// <param name="deviceId">ID of the Tado device to set the child lock for.</param>
        /// <param name="enableChildLock">True to enable child lock, false to disable it.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Boolean indicating if the request was successful.</returns>
        public async Task<bool> SetDeviceChildLockAsync(string deviceId, bool enableChildLock, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Device ID must be provided.", nameof(deviceId));

            return await _httpClient.SendAsync(
                $"devices/{deviceId}/childLock",
                HttpMethod.Put,
                cancellationToken,
                System.Net.HttpStatusCode.NoContent,
                new { childLockEnabled = enableChildLock });
        }

        /// <summary>
        /// Triggers the identify command on a Tado device (displays "Hi" on device).
        /// </summary>
        /// <param name="deviceId">ID of the Tado device to identify.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Boolean indicating if the request was successful.</returns>
        public async Task<bool> SayHiAsync(string deviceId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Device ID must be provided.", nameof(deviceId));

            return await _httpClient.SendAsync(
                $"devices/{deviceId}/identify",
                HttpMethod.Post,
                cancellationToken,
                System.Net.HttpStatusCode.OK,
                new { });
        }

        /// <summary>
        /// Sets the temperature offset in Celsius for a Tado device.
        /// </summary>
        /// <param name="deviceId">The ID of the Tado device to set the offset for.</param>
        /// <param name="temperature">The temperature offset in Celsius.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Boolean indicating if the request was successful.</returns>
        public async Task<bool> SetZoneTemperatureOffsetCelsiusAsync(string deviceId, double temperature, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException("Device ID must be provided.", nameof(deviceId));

            return await _httpClient.SendAsync(
                $"devices/{deviceId}/temperatureOffset",
                HttpMethod.Put,
                cancellationToken,
                System.Net.HttpStatusCode.OK,
                new { celsius = temperature });
        }

        #endregion

    }
}