using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    public class WeatherServiceTests
    {
        [Fact]
        public async Task GetWeatherAsync_ReturnsWeather()
        {
            // Arrange
            var tadoWeather = new TadoWeatherResponse
            {
                Temperature = 15.5, // Celsius
                Humidity = 50
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoWeather);
            var mockAuth = MockTadoAuthService.CreateAuthenticated();

            var service = new TadoWeatherService(mockHttp.Object);

            // Act
            var weather = await service.GetWeatherAsync(1);

            // Assert
            Assert.Equal(15.5, weather.Temperature);
            Assert.Equal(50, weather.Humidity);
        }
    }
}