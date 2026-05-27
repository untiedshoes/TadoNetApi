using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services;

/// <summary>
/// Unit tests for <see cref="WeatherAppService"/>.
/// </summary>
public class WeatherAppServiceTests
{
    /// <summary>
    /// GetWeatherAsync forwards the home ID and cancellation token to the domain service.
    /// </summary>
    [Fact(DisplayName = "GetWeatherAsync forwards home ID and cancellation token")]
    public async Task GetWeatherAsync_ForwardsHomeIdAndCancellationToken()
    {
        var cancellationToken = new CancellationTokenSource().Token;
        var expected = new Weather
        {
            WeatherState = new WeatherState { CurrentType = "SUN", Value = "CLEAR" },
            SolarIntensity = new SolarIntensity { CurrentType = "PERCENTAGE", Percentage = 67 }
        };

        var mockWeatherService = new Mock<IWeatherService>();
        mockWeatherService
            .Setup(s => s.GetWeatherAsync(42, cancellationToken))
            .ReturnsAsync(expected);

        var service = new WeatherAppService(mockWeatherService.Object);

        var result = await service.GetWeatherAsync(42, cancellationToken);

        Assert.Same(expected, result);
        mockWeatherService.Verify(s => s.GetWeatherAsync(42, cancellationToken), Times.Once);
    }

    /// <summary>
    /// GetWeatherAsync uses the default cancellation token when none is provided.
    /// </summary>
    [Fact(DisplayName = "GetWeatherAsync uses default cancellation token when omitted")]
    public async Task GetWeatherAsync_UsesDefaultCancellationToken_WhenOmitted()
    {
        var expected = new Weather();
        var mockWeatherService = new Mock<IWeatherService>();
        mockWeatherService
            .Setup(s => s.GetWeatherAsync(7, CancellationToken.None))
            .ReturnsAsync(expected);

        var service = new WeatherAppService(mockWeatherService.Object);

        var result = await service.GetWeatherAsync(7);

        Assert.Same(expected, result);
        mockWeatherService.Verify(s => s.GetWeatherAsync(7, CancellationToken.None), Times.Once);
    }
}