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

        [Fact(DisplayName = "ResetOpenWindowAsync sends the spec-aligned open window reset command")]
        public async Task ResetOpenWindowAsync_SendsSpecAlignedOpenWindowResetCommand()
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

            await service.ResetOpenWindowAsync(1, 2, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones/2/state/openWindow",
                    HttpMethod.Delete,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    null),
                Times.Once);
        }

        [Fact(DisplayName = "SetZoneDetailsAsync sends the spec-aligned zone details command")]
        public async Task SetZoneDetailsAsync_SendsSpecAlignedZoneDetailsCommand()
        {
            SetZoneDetailsRequest? capturedRequest = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetZoneDetailsRequest, TadoZoneResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneDetailsRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneDetailsRequest, CancellationToken>((_, req, _) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new TadoZoneResponse
                {
                    Id = 2,
                    Name = "Bedroom",
                    CurrentType = "HEATING"
                });

            var service = new TadoZoneService(mockHttp.Object);
            var result = await service.SetZoneDetailsAsync(1, 2, new Zone { Name = "Bedroom" }, CancellationToken.None);

            Assert.NotNull(capturedRequest);
            Assert.Equal("Bedroom", capturedRequest!.Name);
            Assert.Equal("Bedroom", result.Name);

            mockHttp.Verify(c => c.PutAsync<SetZoneDetailsRequest, TadoZoneResponse>(
                "homes/1/zones/2/details",
                It.Is<SetZoneDetailsRequest>(r => r.Name == "Bedroom"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "SetZoneDetailsAsync requires a zone name")]
        public async Task SetZoneDetailsAsync_RequiresZoneName()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetZoneDetailsAsync(1, 2, new Zone { Name = "   " }, CancellationToken.None));

            mockHttp.Verify(c => c.PutAsync<SetZoneDetailsRequest, TadoZoneResponse>(
                It.IsAny<string>(),
                It.IsAny<SetZoneDetailsRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "SetDefaultZoneOverlayAsync sends the spec-aligned default overlay command")]
        public async Task SetDefaultZoneOverlayAsync_SendsSpecAlignedDefaultOverlayCommand()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetDefaultZoneOverlayRequest, TadoDefaultZoneOverlayResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetDefaultZoneOverlayRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetDefaultZoneOverlayRequest, CancellationToken>((_, req, _) =>
                {
                    capturedJson = JsonSerializer.Serialize(req);
                })
                .ReturnsAsync(new TadoDefaultZoneOverlayResponse
                {
                    TerminationCondition = new TadoTerminationResponse
                    {
                        CurrentType = DurationModes.Timer,
                        DurationInSeconds = 900
                    }
                });

            var service = new TadoZoneService(mockHttp.Object);
            var result = await service.SetDefaultZoneOverlayAsync(
                1,
                2,
                new DefaultZoneOverlay
                {
                    TerminationCondition = new Termination
                    {
                        Type = DurationModes.Timer.ToString(),
                        DurationInSeconds = 900
                    }
                },
                CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"terminationCondition\":{", capturedJson);
            Assert.Contains("\"type\":\"TIMER\"", capturedJson);
            Assert.Contains("\"durationInSeconds\":900", capturedJson);
            Assert.Equal(DurationModes.Timer.ToString(), result.TerminationCondition?.Type);
            Assert.Equal(900, result.TerminationCondition?.DurationInSeconds);

            mockHttp.Verify(c => c.PutAsync<SetDefaultZoneOverlayRequest, TadoDefaultZoneOverlayResponse>(
                "homes/1/zones/2/defaultOverlay",
                It.IsAny<SetDefaultZoneOverlayRequest>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "SetDefaultZoneOverlayAsync requires termination details")]
        public async Task SetDefaultZoneOverlayAsync_RequiresTerminationDetails()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetDefaultZoneOverlayAsync(1, 2, new DefaultZoneOverlay(), CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetDefaultZoneOverlayAsync(
                    1,
                    2,
                    new DefaultZoneOverlay
                    {
                        TerminationCondition = new Termination()
                    },
                    CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetDefaultZoneOverlayAsync(
                    1,
                    2,
                    new DefaultZoneOverlay
                    {
                        TerminationCondition = new Termination
                        {
                            Type = DurationModes.Timer.ToString(),
                            DurationInSeconds = 0
                        }
                    },
                    CancellationToken.None));

            mockHttp.Verify(c => c.PutAsync<SetDefaultZoneOverlayRequest, TadoDefaultZoneOverlayResponse>(
                It.IsAny<string>(),
                It.IsAny<SetDefaultZoneOverlayRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "SetZoneOverlaysAsync sends the spec-aligned bulk overlay command")]
        public async Task SetZoneOverlaysAsync_SendsSpecAlignedBulkOverlayCommand()
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
            var zoneOverlays = new Dictionary<int, Overlay>
            {
                [2] = new()
                {
                    Setting = new Setting
                    {
                        DeviceType = DeviceTypes.Heating,
                        Power = PowerStates.On,
                        Temperature = new Temperature { Celsius = 19.5 }
                    },
                    Termination = new Termination
                    {
                        Type = DurationModes.UntilNextTimedEvent.ToString()
                    }
                }
            };

            await service.SetZoneOverlaysAsync(1, zoneOverlays, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"overlays\":[", capturedJson);
            Assert.Contains("\"room\":2", capturedJson);
            Assert.Contains("\"type\":\"HEATING\"", capturedJson);
            Assert.Contains("\"power\":\"ON\"", capturedJson);
            Assert.Contains("\"celsius\":19.5", capturedJson);
            Assert.Contains("\"typeSkillBasedApp\":\"TADO_MODE\"", capturedJson);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/overlay",
                    HttpMethod.Post,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.IsAny<object?>()),
                Times.Once);
        }

        [Fact(DisplayName = "SetZoneOverlaysAsync requires at least one valid overlay")]
        public async Task SetZoneOverlaysAsync_RequiresAtLeastOneValidOverlay()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetZoneOverlaysAsync(1, new Dictionary<int, Overlay>(), CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                service.SetZoneOverlaysAsync(
                    1,
                    new Dictionary<int, Overlay>
                    {
                        [0] = new Overlay()
                    },
                    CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetZoneOverlaysAsync(
                    1,
                    new Dictionary<int, Overlay>
                    {
                        [2] = new Overlay()
                    },
                    CancellationToken.None));

            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "DeleteZoneOverlaysAsync sends the spec-aligned bulk overlay delete command")]
        public async Task DeleteZoneOverlaysAsync_SendsSpecAlignedBulkOverlayDeleteCommand()
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

            await service.DeleteZoneOverlaysAsync(1, new[] { 2, 3 }, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/overlay?rooms=2&rooms=3",
                    HttpMethod.Delete,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    null),
                Times.Once);
        }

        [Fact(DisplayName = "DeleteZoneOverlaysAsync requires at least one positive zone ID")]
        public async Task DeleteZoneOverlaysAsync_RequiresAtLeastOnePositiveZoneId()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.DeleteZoneOverlaysAsync(1, Array.Empty<int>(), CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                service.DeleteZoneOverlaysAsync(1, new[] { 2, 0 }, CancellationToken.None));

            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "SetAwayConfigurationAsync sends the spec-aligned away configuration command")]
        public async Task SetAwayConfigurationAsync_SendsSpecAlignedAwayConfigurationCommand()
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
            var awayConfiguration = new AwayConfiguration
            {
                Type = "HEATING",
                AutoAdjust = false,
                ComfortLevel = "BALANCE",
                Setting = new Setting
                {
                    DeviceType = DeviceTypes.Heating,
                    Power = PowerStates.On,
                    Temperature = new Temperature { Celsius = 15 }
                }
            };

            await service.SetAwayConfigurationAsync(1, 2, awayConfiguration, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"type\":\"HEATING\"", capturedJson);
            Assert.Contains("\"autoAdjust\":false", capturedJson);
            Assert.Contains("\"comfortLevel\":\"BALANCE\"", capturedJson);
            Assert.Contains("\"setting\":{", capturedJson);
            Assert.Contains("\"power\":\"ON\"", capturedJson);
            Assert.Contains("\"celsius\":15", capturedJson);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones/2/schedule/awayConfiguration",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.IsAny<object?>()),
                Times.Once);
        }

        [Fact(DisplayName = "SetAwayConfigurationAsync requires type and setting details")]
        public async Task SetAwayConfigurationAsync_RequiresTypeAndSettingDetails()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetAwayConfigurationAsync(1, 2, new AwayConfiguration(), CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetAwayConfigurationAsync(
                    1,
                    2,
                    new AwayConfiguration
                    {
                        Type = "HEATING"
                    },
                    CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetAwayConfigurationAsync(
                    1,
                    2,
                    new AwayConfiguration
                    {
                        Type = "HEATING",
                        Setting = new Setting()
                    },
                    CancellationToken.None));

            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "SetActiveTimetableTypeAsync sends the spec-aligned active timetable command")]
        public async Task SetActiveTimetableTypeAsync_SendsSpecAlignedActiveTimetableCommand()
        {
            SetActiveTimetableTypeRequest? capturedRequest = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<SetActiveTimetableTypeRequest, TadoTimetableTypeResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetActiveTimetableTypeRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetActiveTimetableTypeRequest, CancellationToken>((_, req, _) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(new TadoTimetableTypeResponse
                {
                    Id = 3,
                    Type = "SEVEN_DAY"
                });

            var service = new TadoZoneService(mockHttp.Object);
            var result = await service.SetActiveTimetableTypeAsync(
                1,
                2,
                new TimetableType { Id = 3, Type = "SEVEN_DAY" },
                CancellationToken.None);

            Assert.NotNull(capturedRequest);
            Assert.Equal(3, capturedRequest!.Id);
            Assert.Equal("SEVEN_DAY", capturedRequest.Type);
            Assert.Equal(3, result.Id);
            Assert.Equal("SEVEN_DAY", result.Type);

            mockHttp.Verify(c => c.PutAsync<SetActiveTimetableTypeRequest, TadoTimetableTypeResponse>(
                "homes/1/zones/2/schedule/activeTimetable",
                It.Is<SetActiveTimetableTypeRequest>(r => r.Id == 3 && r.Type == "SEVEN_DAY"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "SetActiveTimetableTypeAsync requires a positive timetable type ID")]
        public async Task SetActiveTimetableTypeAsync_RequiresPositiveTimetableTypeId()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetActiveTimetableTypeAsync(1, 2, new TimetableType { Id = 0 }, CancellationToken.None));

            mockHttp.Verify(c => c.PutAsync<SetActiveTimetableTypeRequest, TadoTimetableTypeResponse>(
                It.IsAny<string>(),
                It.IsAny<SetActiveTimetableTypeRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "SetTimetableBlocksForDayTypeAsync sends the spec-aligned timetable blocks command")]
        public async Task SetTimetableBlocksForDayTypeAsync_SendsSpecAlignedTimetableBlocksCommand()
        {
            List<SetTimetableBlockRequest>? capturedRequest = null;
            var mockHttp = new Mock<ITadoHttpClient>();

            mockHttp
                .Setup(c => c.PutAsync<List<SetTimetableBlockRequest>, List<TadoTimetableBlockResponse>>(
                    It.IsAny<string>(),
                    It.IsAny<List<SetTimetableBlockRequest>>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, List<SetTimetableBlockRequest>, CancellationToken>((_, req, _) =>
                {
                    capturedRequest = req;
                })
                .ReturnsAsync(
                [
                    new TadoTimetableBlockResponse
                    {
                        DayType = "MONDAY",
                        Start = "06:00",
                        End = "08:00",
                        GeolocationOverride = false,
                        Setting = new TadoSettingResponse
                        {
                            Power = PowerStates.On,
                            Temperature = new TadoTemperatureResponse { Celsius = 20 }
                        }
                    }
                ]);

            var service = new TadoZoneService(mockHttp.Object);
            IReadOnlyList<TimetableBlock> blocks =
            [
                new()
                {
                    DayType = "MONDAY",
                    Start = "06:00",
                    End = "08:00",
                    GeolocationOverride = false,
                    Setting = new Setting
                    {
                        DeviceType = DeviceTypes.Heating,
                        Power = PowerStates.On,
                        Temperature = new Temperature { Celsius = 20 }
                    }
                }
            ];

            var result = await service.SetTimetableBlocksForDayTypeAsync(1, 2, 3, "MONDAY", blocks, CancellationToken.None);

            Assert.NotNull(capturedRequest);
            Assert.Single(capturedRequest!);
            Assert.Equal("MONDAY", capturedRequest[0].DayType);
            Assert.Equal("06:00", capturedRequest[0].Start);
            Assert.Equal(PowerStates.On, capturedRequest[0].Setting.Power);
            Assert.Single(result);
            Assert.Equal("MONDAY", result[0].DayType);
            Assert.Equal(20, result[0].Setting?.Temperature?.Celsius);

            mockHttp.Verify(c => c.PutAsync<List<SetTimetableBlockRequest>, List<TadoTimetableBlockResponse>>(
                "homes/1/zones/2/schedule/timetables/3/blocks/MONDAY",
                It.IsAny<List<SetTimetableBlockRequest>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "SetTimetableBlocksForDayTypeAsync requires day type and valid blocks")]
        public async Task SetTimetableBlocksForDayTypeAsync_RequiresDayTypeAndValidBlocks()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            IReadOnlyList<TimetableBlock> invalidBlocks =
            [
                new()
                {
                    DayType = "MONDAY",
                    Start = "06:00",
                    End = "08:00",
                    Setting = new Setting()
                }
            ];

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetTimetableBlocksForDayTypeAsync(1, 2, 3, " ", invalidBlocks, CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetTimetableBlocksForDayTypeAsync(1, 2, 3, "MONDAY", invalidBlocks, CancellationToken.None));

            mockHttp.Verify(c => c.PutAsync<List<SetTimetableBlockRequest>, List<TadoTimetableBlockResponse>>(
                It.IsAny<string>(),
                It.IsAny<List<SetTimetableBlockRequest>>(),
                It.IsAny<CancellationToken>()), Times.Never);
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
