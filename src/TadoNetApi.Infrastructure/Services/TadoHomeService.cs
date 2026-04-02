using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IHomeService using the Tado API.
/// </summary>
public class TadoHomeService : IHomeService
{
    private readonly ITadoHttpClient _httpClient;

    public TadoHomeService(ITadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<House?> GetHomeAsync(int homeId, CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoHouseResponse>($"homes/{homeId}", cancellationToken);

        return dto == null ? null : HouseMapper.ToDomain(dto);
    }

    /// <inheritdoc/>
    public async Task<HomeState?> GetHomeStateAsync(int homeId, CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoHomeStateResponse>($"homes/{homeId}/state", cancellationToken);

        return dto == null ? null : dto.ToDomain();
    }

    /// <inheritdoc/>
    public async Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken = default)
    {
        var request = new SetHomePresenceRequest
        {
            presence = presence
        };

        await _httpClient.PostAsync<SetHomePresenceRequest, object>(
            $"homes/{homeId}/presence",
            request,
            cancellationToken
        );
    }
}