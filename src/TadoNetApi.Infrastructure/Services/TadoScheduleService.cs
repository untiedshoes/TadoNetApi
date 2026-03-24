using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IScheduleService using the Tado API.
/// </summary>
public class TadoScheduleService : IScheduleService
{
    private readonly TadoHttpClient _httpClient;

    public TadoScheduleService(TadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<List<Schedule>> GetZoneScheduleAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
    {
        var dtos = await _httpClient.GetAsync<List<TadoScheduleResponse>>(
            $"homes/{homeId}/zones/{zoneId}/schedule",
            cancellationToken
        );

        return dtos == null ? new List<Schedule>() : ScheduleMapper.ToDomainList(dtos, zoneId);
    }

    /// <inheritdoc/>
    public async Task SetZoneScheduleEntryAsync(int homeId, int zoneId, Schedule entry, CancellationToken cancellationToken = default)
    {
        var request = ScheduleMapper.ToRequest(entry);
        await _httpClient.PostAsync<TadoScheduleRequest, object>(
            $"homes/{homeId}/zones/{zoneId}/schedule",
            request,
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public async Task<string> GetZoneProgramAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync<TadoProgramResponse>(
            $"homes/{homeId}/zones/{zoneId}/program",
            cancellationToken
        );

        return response?.Name ?? "Unknown";
    }
}