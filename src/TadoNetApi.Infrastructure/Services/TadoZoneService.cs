using System.Net;
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
                var response = await _httpClient.GetAsync<List<TadoCapabilityResponse>>(
                    $"homes/{homeId}/zones/{zoneId}/capabilities",
                    cancellationToken) ?? new List<TadoCapabilityResponse>();

                return response.Select(c => c.ToDomain()).ToList();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve zone capabilities: {ex.Message}");
            }
        }
    }
}