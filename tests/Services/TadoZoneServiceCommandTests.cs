using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    /// <summary>
    /// Unit tests for command-style operations in <see cref="TadoZoneService"/>.
    /// </summary>
    public class TadoZoneServiceCommandTests
    {
        [Fact(DisplayName = "SetHeatingTemperatureCelsiusAsync falls back to manual when timer is missing")]
        public async Task SetHeatingTemperatureCelsiusAsync_TimerWithoutTimer_FallsBackToManual()
        {
            // Arrange
            SetZoneTemperatureRequest? capturedRequest = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) => capturedRequest = req)
                .ReturnsAsync(new TadoZoneSummaryResponse
                {
                    Termination = new TadoTerminationResponse { CurrentType = DurationModes.UntilNextManualChange }
                });

            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var response = await service.SetHeatingTemperatureCelsiusAsync(
                homeId: 1,
                zoneId: 2,
                temperature: 21.0,
                durationMode: DurationModes.Timer,
                timer: null,
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(DurationModes.UntilNextManualChange, capturedRequest!.Termination.CurrentType);
            Assert.Null(capturedRequest.Termination.DurationInSeconds);
            Assert.Equal(PowerStates.On, capturedRequest.Setting.Power);
            Assert.Equal(21.0, capturedRequest.Setting.Temperature?.Celsius);
            Assert.Equal(DurationModes.UntilNextManualChange.ToString(), response?.Termination?.Type);

            mockHttp.Verify(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                "homes/1/zones/2/overlay",
                It.IsAny<SetZoneTemperatureRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "SetHeatingTemperatureCelsiusAsync timer mode sets durationInSeconds")]
        public async Task SetHeatingTemperatureCelsiusAsync_TimerWithDuration_SetsDurationInSeconds()
        {
            // Arrange
            SetZoneTemperatureRequest? capturedRequest = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) => capturedRequest = req)
                .ReturnsAsync(new TadoZoneSummaryResponse
                {
                    Termination = new TadoTerminationResponse
                    {
                        CurrentType = DurationModes.Timer,
                        DurationInSeconds = 900
                    }
                });

            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var response = await service.SetHeatingTemperatureCelsiusAsync(
                homeId: 1,
                zoneId: 2,
                temperature: 20.5,
                durationMode: DurationModes.Timer,
                timer: TimeSpan.FromMinutes(15),
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(capturedRequest);
            Assert.Equal(DurationModes.Timer, capturedRequest!.Termination.CurrentType);
            Assert.Equal(900, capturedRequest.Termination.DurationInSeconds);
            Assert.Equal(DurationModes.Timer.ToString(), response?.Termination?.Type);
            Assert.Equal(900, response?.Termination?.DurationInSeconds);
        }
    }
}
