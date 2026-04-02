using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

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

        /// <summary>
        /// Retrieves all devices for the specified home.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A read-only list of <see cref="Device"/>.</returns>
        /// <exception cref="TadoApiException">Thrown if the API request fails.</exception>
        public async Task<IReadOnlyList<Device>> GetDevicesAsync(int homeId, CancellationToken cancellationToken = default)
        {
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
        /// Retrieves a single device by home and device ID.
        /// </summary>
        /// <param name="homeId">The home ID.</param>
        /// <param name="deviceId">The device ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="Device"/>.</returns>
        /// <exception cref="TadoApiException">Thrown if the device is not found or request fails.</exception>
        public async Task<Device> GetDeviceAsync(int homeId, int deviceId, CancellationToken cancellationToken = default)
        {
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
        /// Retrieves the settings for a specific mobile device.
        /// </summary>
        /// <param name="homeId">The ID of the home.</param>
        /// <param name="mobileDeviceId">The ID of the mobile device.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The <see cref="Settings"/> for the mobile device.</returns>
        /// <exception cref="TadoApiException">Thrown if the device is not found or request fails.</exception>
        public async Task<Settings> GetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default)
        {
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
    }
}