using System.Net;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Validation;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IWeatherService using the Tado API.
/// </summary>
public class TadoWeatherService : IWeatherService
{
    private readonly ITadoHttpClient _httpClient;

    public TadoWeatherService(ITadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #region Data Retrieval

    /// <inheritdoc/>
    public async Task<Weather> GetWeatherAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        var response = await _httpClient.GetAsync<TadoWeatherResponse>($"homes/{homeId}/weather", cancellationToken);
        if (response == null)
            throw new TadoApiException(HttpStatusCode.NotFound, $"Weather not found");

        return WeatherMapper.ToDomain(response);
    }

    #endregion

}