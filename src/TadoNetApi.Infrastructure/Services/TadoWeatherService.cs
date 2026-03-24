using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IWeatherService using the Tado API.
/// </summary>
public class TadoWeatherService : IWeatherService
{
    private readonly TadoHttpClient _httpClient;

    public TadoWeatherService(TadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<Weather> GetWeatherAsync(int homeId, CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoWeatherResponse>($"homes/{homeId}/weather", cancellationToken);
        if (dto == null)
            throw new Exception("Weather not found");

        return WeatherMapper.ToDomain(dto);
    }
}