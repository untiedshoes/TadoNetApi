using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services
{
    /// <summary>
    /// Application-level service for weather operations.
    /// </summary>
    public class WeatherAppService
    {
        private readonly IWeatherService _weatherService;

        /// <summary>
        /// Initializes a new instance of <see cref="WeatherAppService"/>.
        /// </summary>
        /// <param name="weatherService">The domain weather service to use.</param>
        public WeatherAppService(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        /// <summary>
        /// Retrieves weather data for a specified home.
        /// </summary>
        /// <param name="homeId">The ID of the home whose weather should be retrieved.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The current weather for the home.</returns>
        public Task<Weather> GetWeatherAsync(int homeId, CancellationToken cancellationToken = default)
            => _weatherService.GetWeatherAsync(homeId, cancellationToken);
    }
}