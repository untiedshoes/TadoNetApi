using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IZoneService using the Tado API.
/// </summary>
public class TadoZoneService : IZoneService
{
    private readonly TadoHttpClient _httpClient;

    public TadoZoneService(TadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<List<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default)
    {
        var dtos = await _httpClient.GetAsync<List<TadoZoneResponse>>($"homes/{homeId}/zones", cancellationToken);
        return dtos == null ? new List<Zone>() : ZoneMapper.ToDomainList(dtos);
    }

    /// <inheritdoc/>
    public async Task<Zone> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoZoneResponse>($"homes/{homeId}/zones/{zoneId}", cancellationToken);
        if (dto == null)
            throw new Exception("Zone not found");

        return ZoneMapper.ToDomain(dto);
    }

    /// <inheritdoc/>
    public async Task<ZoneState> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoZoneResponse>($"homes/{homeId}/zones/{zoneId}", cancellationToken);
        if (dto == null)
            throw new Exception("Zone not found");

        return ZoneMapper.ToDomainState(dto);
    }

    /// <inheritdoc/>
    public async Task SetZoneTemperatureAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
    {
        var request = new SetZoneTemperatureRequest
        {
            Temperature = temperature
        };

        await _httpClient.PostAsync<SetZoneTemperatureRequest, object>(
            $"homes/{homeId}/zones/{zoneId}/overlay",
            request,
            cancellationToken
        );
    }
}