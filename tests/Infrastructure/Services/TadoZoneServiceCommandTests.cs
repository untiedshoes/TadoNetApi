using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using Moq;
using TadoNetApi.Domain.Entities;
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
        [Fact(DisplayName = "DeleteZoneOverlayAsync sends the spec-aligned overlay delete command")]
        public async Task DeleteZoneOverlayAsync_SendsSpecAlignedOverlayDeleteCommand()
        {
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<System.Net.HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoZoneService(mockHttp.Object);

            var deleted = await service.DeleteZoneOverlayAsync(1, 2, CancellationToken.None);

            Assert.True(deleted);
            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones/2/overlay",
                    HttpMethod.Delete,
                    It.IsAny<CancellationToken>(),
                    System.Net.HttpStatusCode.OK,
                    null),
                Times.Once);
        }

        [Fact(DisplayName = "SetHeatingCircuitAsync sends the spec-aligned heating circuit command")]
        public async Task SetHeatingCircuitAsync_SendsSpecAlignedHeatingCircuitCommand()
        {
            SetHeatingCircuitRequest? capturedRequest = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetHeatingCircuitRequest?, TadoZoneControlResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetHeatingCircuitRequest?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetHeatingCircuitRequest?, CancellationToken>((_, req, _) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new TadoZoneControlResponse
                {
                    Type = "HEATING",
                    HeatingCircuit = 3
                });

            var service = new TadoZoneService(mockHttp.Object);

            var result = await service.SetHeatingCircuitAsync(1, 2, 3, CancellationToken.None);

            Assert.NotNull(capturedRequest);
            Assert.Equal(3, capturedRequest!.CircuitNumber);
            Assert.Equal(3, result.HeatingCircuit);
            mockHttp.Verify(c => c.PutAsync<SetHeatingCircuitRequest?, TadoZoneControlResponse>(
                "homes/1/zones/2/control/heatingCircuit",
                It.Is<SetHeatingCircuitRequest?>(r => r != null && r.CircuitNumber == 3),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "SetHeatingCircuitAsync sends an empty body to remove the heating circuit assignment")]
        public async Task SetHeatingCircuitAsync_NullCircuit_SendsEmptyBody()
        {
            SetHeatingCircuitRequest? capturedRequest = new SetHeatingCircuitRequest();
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetHeatingCircuitRequest?, TadoZoneControlResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetHeatingCircuitRequest?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetHeatingCircuitRequest?, CancellationToken>((_, req, _) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new TadoZoneControlResponse
                {
                    Type = "HEATING",
                    HeatingCircuit = null
                });

            var service = new TadoZoneService(mockHttp.Object);

            var result = await service.SetHeatingCircuitAsync(1, 2, null, CancellationToken.None);

            Assert.Null(capturedRequest);
            Assert.Null(result.HeatingCircuit);
            mockHttp.Verify(c => c.PutAsync<SetHeatingCircuitRequest?, TadoZoneControlResponse>(
                "homes/1/zones/2/control/heatingCircuit",
                null,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "SetHeatingCircuitAsync rejects non-positive circuit numbers")]
        public async Task SetHeatingCircuitAsync_RejectsNonPositiveCircuitNumbers()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                service.SetHeatingCircuitAsync(1, 2, 0, CancellationToken.None));

            mockHttp.Verify(c => c.PutAsync<SetHeatingCircuitRequest?, TadoZoneControlResponse>(
                It.IsAny<string>(),
                It.IsAny<SetHeatingCircuitRequest?>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "SetOpenWindowDetectionAsync sends the spec-aligned open window detection command")]
        public async Task SetOpenWindowDetectionAsync_SendsSpecAlignedOpenWindowDetectionCommand()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .Callback<string, HttpMethod, CancellationToken, HttpStatusCode, object?>((_, _, _, _, body) =>
                {
                    capturedJson = body == null ? null : JsonSerializer.Serialize(body);
                })
                .ReturnsAsync(true);

            var service = new TadoZoneService(mockHttp.Object);
            var settings = new OpenWindowDetection
            {
                Enabled = true,
                TimeoutInSeconds = 900
            };

            await service.SetOpenWindowDetectionAsync(1, 2, settings, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"roomId\":2", capturedJson);
            Assert.Contains("\"enabled\":true", capturedJson);
            Assert.Contains("\"timeoutInSeconds\":900", capturedJson);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones/2/openWindowDetection",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.IsAny<object?>()),
                Times.Once);
        }

        [Fact(DisplayName = "SetOpenWindowDetectionAsync requires enabled state and timeout")]
        public async Task SetOpenWindowDetectionAsync_RequiresEnabledStateAndTimeout()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetOpenWindowDetectionAsync(1, 2, new OpenWindowDetection { TimeoutInSeconds = 900 }, CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetOpenWindowDetectionAsync(1, 2, new OpenWindowDetection { Enabled = true }, CancellationToken.None));

            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "SetOpenWindowDetectionAsync rejects negative timeout")]
        public async Task SetOpenWindowDetectionAsync_RejectsNegativeTimeout()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                service.SetOpenWindowDetectionAsync(1, 2, new OpenWindowDetection { Enabled = true, TimeoutInSeconds = -1 }, CancellationToken.None));

            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "ActivateOpenWindowAsync sends the spec-aligned open window activation command")]
        public async Task ActivateOpenWindowAsync_SendsSpecAlignedOpenWindowActivationCommand()
        {
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoZoneService(mockHttp.Object);

            await service.ActivateOpenWindowAsync(1, 2, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones/2/state/openWindow/activate",
                    HttpMethod.Post,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    null),
                Times.Once);
        }

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

        [Fact(DisplayName = "SetHeatingTemperatureFahrenheitAsync sends a Fahrenheit heating overlay")]
        public async Task SetHeatingTemperatureFahrenheitAsync_SendsFahrenheitHeatingOverlay()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) =>
                {
                    capturedJson = JsonSerializer.Serialize(req);
                })
                .ReturnsAsync(new TadoZoneSummaryResponse());

            var service = new TadoZoneService(mockHttp.Object);

            await service.SetHeatingTemperatureFahrenheitAsync(1, 2, 72.5, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"type\":\"HEATING\"", capturedJson);
            Assert.Contains("\"power\":\"ON\"", capturedJson);
            Assert.Contains("\"fahrenheit\":72.5", capturedJson);
            Assert.DoesNotContain("\"celsius\"", capturedJson);
        }

        [Fact(DisplayName = "SetHotWaterTemperatureCelsiusAsync sends a Celsius hot water overlay")]
        public async Task SetHotWaterTemperatureCelsiusAsync_SendsCelsiusHotWaterOverlay()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) =>
                {
                    capturedJson = JsonSerializer.Serialize(req);
                })
                .ReturnsAsync(new TadoZoneSummaryResponse());

            var service = new TadoZoneService(mockHttp.Object);

            await service.SetHotWaterTemperatureCelsiusAsync(1, 2, 55.0, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"type\":\"HOT_WATER\"", capturedJson);
            Assert.Contains("\"power\":\"ON\"", capturedJson);
            Assert.Contains("\"celsius\":55", capturedJson);
            Assert.DoesNotContain("\"fahrenheit\"", capturedJson);
        }

        [Fact(DisplayName = "SetHotWaterTemperatureFahrenheitAsync sends a Fahrenheit hot water overlay")]
        public async Task SetHotWaterTemperatureFahrenheitAsync_SendsFahrenheitHotWaterOverlay()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) =>
                {
                    capturedJson = JsonSerializer.Serialize(req);
                })
                .ReturnsAsync(new TadoZoneSummaryResponse());

            var service = new TadoZoneService(mockHttp.Object);

            await service.SetHotWaterTemperatureFahrenheitAsync(1, 2, 131.0, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"type\":\"HOT_WATER\"", capturedJson);
            Assert.Contains("\"power\":\"ON\"", capturedJson);
            Assert.Contains("\"fahrenheit\":131", capturedJson);
            Assert.DoesNotContain("\"celsius\"", capturedJson);
        }

        [Fact(DisplayName = "SwitchHeatingOffAsync sends an off heating overlay")]
        public async Task SwitchHeatingOffAsync_SendsOffHeatingOverlay()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) =>
                {
                    capturedJson = JsonSerializer.Serialize(req);
                })
                .ReturnsAsync(new TadoZoneSummaryResponse());

            var service = new TadoZoneService(mockHttp.Object);

            await service.SwitchHeatingOffAsync(1, 2, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"type\":\"HEATING\"", capturedJson);
            Assert.Contains("\"power\":\"OFF\"", capturedJson);
            Assert.DoesNotContain("\"temperature\"", capturedJson);
        }

        [Fact(DisplayName = "SwitchHotWaterOffAsync sends an off hot water overlay")]
        public async Task SwitchHotWaterOffAsync_SendsOffHotWaterOverlay()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneTemperatureRequest, TadoZoneSummaryResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneTemperatureRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneTemperatureRequest, CancellationToken>((_, req, _) =>
                {
                    capturedJson = JsonSerializer.Serialize(req);
                })
                .ReturnsAsync(new TadoZoneSummaryResponse());

            var service = new TadoZoneService(mockHttp.Object);

            await service.SwitchHotWaterOffAsync(1, 2, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"type\":\"HOT_WATER\"", capturedJson);
            Assert.Contains("\"power\":\"OFF\"", capturedJson);
            Assert.DoesNotContain("\"temperature\"", capturedJson);
        }
    }
}
