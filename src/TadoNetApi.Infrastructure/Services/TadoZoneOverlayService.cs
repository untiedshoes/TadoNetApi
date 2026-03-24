using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation for managing overlays in zones using Tado API.
/// </summary>
public class TadoZoneOverlayService : IZoneOverlayService
{
    private readonly TadoHttpClient _httpClient;

    public TadoZoneOverlayService(TadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task SetZoneTemperatureAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
    {
        var request = new SetZoneTemperatureRequest
        {
            Temperature = temperature,
            Type = "MANUAL"
        };

        await _httpClient.PostAsync<SetZoneTemperatureRequest, object>(
            $"homes/{homeId}/zones/{zoneId}/overlay",
            request,
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public async Task<Overlay> GetZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoOverlayResponse>(
            $"homes/{homeId}/zones/{zoneId}/overlay",
            cancellationToken
        );

        if (dto == null)
            throw new Exception("Overlay not found");

        return OverlayMapper.ToDomain(dto);
    }
}