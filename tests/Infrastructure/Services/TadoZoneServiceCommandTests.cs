using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Moq;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    /// <summary>
    /// Unit tests for command-style operations in <see cref="TadoZoneService"/>.
    /// </summary>
    public class TadoZoneServiceCommandTests
    {
        [Fact(DisplayName = "CreateZoneAsync sends the spec-aligned zone creation command")]
        public async Task CreateZoneAsync_SendsSpecAlignedZoneCreationCommand()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<System.Net.HttpStatusCode>(),
                    It.IsAny<object?>()))
                .Callback<string, HttpMethod, CancellationToken, System.Net.HttpStatusCode, object?>((_, _, _, _, body) =>
                {
                    capturedJson = body == null ? null : JsonSerializer.Serialize(body);
                })
                .ReturnsAsync(true);

            var service = new TadoZoneService(mockHttp.Object);

            await service.CreateZoneAsync(1, "HEATING", ["SU1234567890", "VA1234567890"], true, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"type\":\"IMPLICIT_CONTROL\"", capturedJson);
            Assert.Contains("\"zoneType\":\"HEATING\"", capturedJson);
            Assert.Contains("\"serialNo\":\"SU1234567890\"", capturedJson);
            Assert.Contains("\"serialNo\":\"VA1234567890\"", capturedJson);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones?force=true",
                    HttpMethod.Post,
                    It.IsAny<CancellationToken>(),
                    System.Net.HttpStatusCode.Created,
                    It.IsAny<object?>()),
                Times.Once);
        }

        [Fact(DisplayName = "CreateZoneAsync rejects missing device serial numbers")]
        public async Task CreateZoneAsync_RejectsMissingDeviceSerialNumbers()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.CreateZoneAsync(1, "HEATING", [], null, CancellationToken.None));

            Assert.Equal("deviceSerialNumbers", exception.ParamName);
            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<System.Net.HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "SetHeatingTemperatureCelsiusAsync falls back to manual when timer is missing")]
        public async Task SetHeatingTemperatureCelsiusAsync_TimerWithoutTimer_FallsBackToManual()
        {
            // Arrange
            SetZoneTemperatureRequest? capturedRequest = null;
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) =>
                {
                    capturedRequest = req;
                    capturedJson = JsonSerializer.Serialize(req);
                })
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
            Assert.NotNull(capturedJson);
            Assert.Contains("\"typeSkillBasedApp\":\"MANUAL\"", capturedJson);
            Assert.DoesNotContain("\"type\":\"MANUAL\"", capturedJson);
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
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) =>
                {
                    capturedRequest = req;
                    capturedJson = JsonSerializer.Serialize(req);
                })
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
            Assert.NotNull(capturedJson);
            Assert.Contains("\"typeSkillBasedApp\":\"TIMER\"", capturedJson);
            Assert.DoesNotContain("\"type\":\"TIMER\"", capturedJson);
            Assert.Contains("\"durationInSeconds\":900", capturedJson);
            Assert.Equal(DurationModes.Timer.ToString(), response?.Termination?.Type);
            Assert.Equal(900, response?.Termination?.DurationInSeconds);
        }
    }
}
