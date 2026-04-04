using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Infrastructure.Validation;

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

    #region Data Retrieval

    /// <inheritdoc/>
    public async Task<House?> GetHomeAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        var dto = await _httpClient.GetAsync<TadoHouseResponse>($"homes/{homeId}", cancellationToken);

        return dto == null ? null : HouseMapper.ToDomain(dto);
    }

    /// <inheritdoc/>
    public async Task<HomeState?> GetHomeStateAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        var dto = await _httpClient.GetAsync<TadoHomeStateResponse>($"homes/{homeId}/state", cancellationToken);

        return dto == null ? null : dto.ToDomain();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<User>> GetUsersAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<List<TadoUserResponse>>(
                $"homes/{homeId}/users",
                cancellationToken) ?? new List<TadoUserResponse>();

            return UserMapper.ToDomainList(response);
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve users: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<AirComfort> GetAirComfortAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<TadoAirComfortResponse>(
                $"homes/{homeId}/airComfort",
                cancellationToken);

            if (response == null)
                throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                    $"Air comfort for home {homeId} not found.");

            return response.ToDomain();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve air comfort: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<IncidentDetection> GetIncidentDetectionAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<TadoIncidentDetectionResponse>(
                $"homes/{homeId}/incidentDetection",
                cancellationToken);

            if (response == null)
                throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                    $"Incident detection for home {homeId} not found.");

            return response.ToDomain();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve incident detection: {ex.Message}");
        }
    }

    #endregion

    #region Send Commands

    /// <inheritdoc/>
    public async Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

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

    #endregion

}