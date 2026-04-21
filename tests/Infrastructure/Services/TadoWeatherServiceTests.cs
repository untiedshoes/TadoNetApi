using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoWeatherServiceTests
    {
        /// <summary>
        /// Get Weather Async_ Returns Weather.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_ReturnsWeather()
        {
            // Arrange
            var tadoWeather = new TadoWeatherResponse
            {
                OutsideTemperature = new TadoOutsideTemperatureResponse { Celsius = 15.5 }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoWeather);
            var mockAuth = MockTadoAuthService.CreateAuthenticated();

            var service = new TadoWeatherService(mockHttp.Object);

            // Act
            var weather = await service.GetWeatherAsync(1);

            // Assert
            Assert.Equal(15.5, weather.OutsideTemperature?.Celsius);
        }

        /// <summary>
        /// GetWeatherAsync throws TadoApiException when API returns Unauthorized.
        /// </summary>
        [Fact(DisplayName = "GetWeatherAsync throws TadoApiException when API returns Unauthorized")]
        public async Task GetWeatherAsync_ShouldThrowTadoApiException_WhenApiReturnsUnauthorized()
        {
            var mockHttp = MockTadoHttpClient.CreateGet<TadoWeatherResponse>(
                returnValue: null!,
                transientFailures: int.MaxValue,
                transientException: new TadoApiException(HttpStatusCode.Unauthorized, "Unauthorized"));

            var service = new TadoWeatherService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetWeatherAsync(homeId: 1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        }

        /// <summary>
        /// GetWeatherAsync throws TadoApiException with ServiceUnavailable when network fails.
        /// </summary>
        [Fact(DisplayName = "GetWeatherAsync throws TadoApiException with ServiceUnavailable when network fails")]
        public async Task GetWeatherAsync_ShouldThrowTadoApiException_WhenNetworkFails()
        {
            var mockHttp = MockTadoHttpClient.CreateGet<TadoWeatherResponse>(
                returnValue: null!,
                transientFailures: int.MaxValue,
                transientException: new HttpRequestException("Network failed"));

            var service = new TadoWeatherService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetWeatherAsync(homeId: 1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }
    }
}