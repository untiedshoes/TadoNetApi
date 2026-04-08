using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoHomeServiceTests
    {
        [Fact(DisplayName = "GetHomeAsync returns mapped house with incident detection")]
        public async Task GetHomeAsync_ReturnsMappedHouseWithIncidentDetection()
        {
            var response = new TadoHouseResponse
            {
                Id = 1,
                Name = "My Home",
                DateTimeZone = "Europe/London",
                DateCreated = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                TemperatureUnit = "C",
                InstallationCompleted = true,
                Partner = new object(),
                SimpleSmartScheduleEnabled = true,
                AwayRadiusInMeters = 400,
                License = "PREMIUM",
                ChristmasModeEnabled = false,
                IncidentDetection = new TadoIncidentDetectionResponse
                {
                    Enabled = true,
                    Supported = true
                },
                ContactDetails = new TadoContactDetailsResponse(),
                Address = new TadoAddressResponse(),
                Geolocation = new TadoGeolocationResponse()
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            var home = await service.GetHomeAsync(homeId: 1, CancellationToken.None);

            Assert.NotNull(home);
            Assert.Equal("My Home", home!.Name);
            Assert.True(home.IncidentDetection?.Enabled);
            Assert.True(home.IncidentDetection?.Supported);
        }

        [Fact(DisplayName = "GetUsersAsync returns mapped users")]
        public async Task GetUsersAsync_ReturnsMappedUsers()
        {
            // Arrange
            var userResponses = new List<TadoUserResponse>
            {
                new()
                {
                    Id = "user-1",
                    Name = "Alice Example",
                    Email = "alice@example.com",
                    Username = "alice"
                },
                new()
                {
                    Id = "user-2",
                    Name = "Bob Example",
                    Email = "bob@example.com",
                    Username = "bob"
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(userResponses);
            var service = new TadoHomeService(mockHttp.Object);

            // Act
            var users = await service.GetUsersAsync(homeId: 1, CancellationToken.None);

            // Assert
            Assert.Equal(2, users.Count);
            Assert.Equal("user-1", users[0].Id);
            Assert.Equal("Alice Example", users[0].Name);
            Assert.Equal("alice@example.com", users[0].Email);
            Assert.Equal("alice", users[0].Username);
            Assert.Equal("user-2", users[1].Id);
            Assert.Equal("Bob Example", users[1].Name);
        }

        [Fact(DisplayName = "GetAirComfortAsync returns mapped air comfort")]
        public async Task GetAirComfortAsync_ReturnsMappedAirComfort()
        {
            // Arrange
            var response = new TadoAirComfortResponse
            {
                Freshness = new TadoAirComfortFreshnessResponse
                {
                    Value = "FRESH",
                    LastOpenWindow = new DateTime(2025, 1, 22, 21, 0, 0, DateTimeKind.Utc)
                },
                AcPoweredOn = false,
                LastAcPowerOff = new DateTime(2025, 1, 22, 21, 0, 0, DateTimeKind.Utc),
                Comfort =
                [
                    new TadoAirComfortComfortResponse
                    {
                        RoomId = 3,
                        TemperatureLevel = "COMFY",
                        HumidityLevel = "DRY",
                        Coordinate = new TadoAirComfortCoordinateResponse
                        {
                            Radial = 0.22,
                            Angular = 76
                        }
                    }
                ]
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            // Act
            var airComfort = await service.GetAirComfortAsync(homeId: 1, CancellationToken.None);

            // Assert
            Assert.Equal("FRESH", airComfort.Freshness?.Value);
            Assert.Equal(new DateTime(2025, 1, 22, 21, 0, 0, DateTimeKind.Utc), airComfort.Freshness?.LastOpenWindow);
            Assert.False(airComfort.AcPoweredOn);
            Assert.Equal(new DateTime(2025, 1, 22, 21, 0, 0, DateTimeKind.Utc), airComfort.LastAcPowerOff);
            Assert.Single(airComfort.Comfort!);
            Assert.Equal(3, airComfort.Comfort![0].RoomId);
            Assert.Equal("COMFY", airComfort.Comfort[0].TemperatureLevel);
            Assert.Equal("DRY", airComfort.Comfort[0].HumidityLevel);
            Assert.Equal(0.22, airComfort.Comfort[0].Coordinate?.Radial);
            Assert.Equal(76, airComfort.Comfort[0].Coordinate?.Angular);
        }

        [Fact(DisplayName = "GetInstallationsAsync returns mapped installations")]
        public async Task GetInstallationsAsync_ReturnsMappedInstallations()
        {
            var response = new List<TadoInstallationResponse>
            {
                new()
                {
                    Id = 101,
                    CurrentType = "HEATING",
                    Revision = 7,
                    State = "COMPLETED",
                    Devices =
                    [
                        new TadoDeviceResponse
                        {
                            SerialNo = "VA1234567890",
                            ShortSerialNo = "VA1234567890",
                            DeviceType = "VA01"
                        }
                    ]
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            var installations = await service.GetInstallationsAsync(homeId: 1, CancellationToken.None);

            Assert.Single(installations);
            Assert.Equal(101, installations[0].Id);
            Assert.Equal("HEATING", installations[0].CurrentType);
            Assert.Equal(7, installations[0].Revision);
            Assert.Equal("COMPLETED", installations[0].State);
            Assert.Single(installations[0].Devices);
            Assert.Equal("VA1234567890", installations[0].Devices[0].SerialNo);
        }

        [Fact(DisplayName = "GetInstallationAsync returns mapped installation")]
        public async Task GetInstallationAsync_ReturnsMappedInstallation()
        {
            var response = new TadoInstallationResponse
            {
                Id = 101,
                CurrentType = "HEATING",
                Revision = 7,
                State = "COMPLETED",
                Devices =
                [
                    new TadoDeviceResponse
                    {
                        SerialNo = "VA1234567890",
                        ShortSerialNo = "VA1234567890",
                        DeviceType = "VA01"
                    }
                ]
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            var installation = await service.GetInstallationAsync(homeId: 1, installationId: 101, CancellationToken.None);

            Assert.NotNull(installation);
            Assert.Equal(101, installation!.Id);
            Assert.Equal("HEATING", installation.CurrentType);
            Assert.Equal(7, installation.Revision);
            Assert.Equal("COMPLETED", installation.State);
            Assert.Single(installation.Devices);
            Assert.Equal("VA1234567890", installation.Devices[0].SerialNo);
        }

        [Fact(DisplayName = "GetIncidentDetectionAsync returns mapped incident detection")]
        public async Task GetIncidentDetectionAsync_ReturnsMappedIncidentDetection()
        {
            // Arrange
            var response = new TadoIncidentDetectionResponse
            {
                Enabled = true,
                Supported = false
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            // Act
            var incidentDetection = await service.GetIncidentDetectionAsync(homeId: 1, CancellationToken.None);

            // Assert
            Assert.True(incidentDetection.Enabled);
            Assert.False(incidentDetection.Supported);
        }

        [Fact(DisplayName = "GetHeatingCircuitsAsync returns mapped heating circuits")]
        public async Task GetHeatingCircuitsAsync_ReturnsMappedHeatingCircuits()
        {
            var response = new List<TadoHeatingCircuitResponse>
            {
                new()
                {
                    Number = 1,
                    DriverSerialNo = "BR3209250550",
                    DriverShortSerialNo = "BR3209250550"
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            var circuits = await service.GetHeatingCircuitsAsync(homeId: 1, CancellationToken.None);

            Assert.Single(circuits);
            Assert.Equal(1, circuits[0].Number);
            Assert.Equal("BR3209250550", circuits[0].DriverSerialNo);
            Assert.Equal("BR3209250550", circuits[0].DriverShortSerialNo);
        }

        [Fact(DisplayName = "GetHeatingSystemAsync returns mapped heating system")]
        public async Task GetHeatingSystemAsync_ReturnsMappedHeatingSystem()
        {
            var response = new TadoHeatingSystemResponse
            {
                Boiler = new TadoBoilerResponse
                {
                    Present = true,
                    Id = 2699,
                    Found = true
                },
                UnderfloorHeating = new TadoUnderfloorHeatingResponse
                {
                    Present = false
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            var heatingSystem = await service.GetHeatingSystemAsync(homeId: 1, CancellationToken.None);

            Assert.NotNull(heatingSystem);
            Assert.True(heatingSystem.Boiler?.Present);
            Assert.Equal(2699, heatingSystem.Boiler?.Id);
            Assert.True(heatingSystem.Boiler?.Found);
            Assert.False(heatingSystem.UnderfloorHeating?.Present);
        }

        [Fact(DisplayName = "GetFlowTemperatureOptimisationAsync returns mapped flow temperature optimisation")]
        public async Task GetFlowTemperatureOptimisationAsync_ReturnsMappedFlowTemperatureOptimisation()
        {
            var response = new TadoFlowTemperatureOptimisationResponse
            {
                HasMultipleBoilerControlDevices = false,
                MaxFlowTemperature = 50,
                MaxFlowTemperatureConstraints = new TadoFlowTemperatureOptimisationConstraintsResponse
                {
                    Min = 30,
                    Max = 80
                },
                AutoAdaptation = new TadoFlowTemperatureOptimisationAutoAdaptationResponse
                {
                    Enabled = true,
                    MaxFlowTemperature = 45
                },
                OpenThermDeviceSerialNumber = "BR1234567890"
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoHomeService(mockHttp.Object);

            var flowTemperatureOptimisation = await service.GetFlowTemperatureOptimisationAsync(homeId: 1, CancellationToken.None);

            Assert.NotNull(flowTemperatureOptimisation);
            Assert.False(flowTemperatureOptimisation.HasMultipleBoilerControlDevices);
            Assert.Equal(50, flowTemperatureOptimisation.MaxFlowTemperature);
            Assert.Equal(30, flowTemperatureOptimisation.MaxFlowTemperatureConstraints?.Min);
            Assert.Equal(80, flowTemperatureOptimisation.MaxFlowTemperatureConstraints?.Max);
            Assert.True(flowTemperatureOptimisation.AutoAdaptation?.Enabled);
            Assert.Equal(45, flowTemperatureOptimisation.AutoAdaptation?.MaxFlowTemperature);
            Assert.Equal("BR1234567890", flowTemperatureOptimisation.OpenThermDeviceSerialNumber);
        }

        [Fact(DisplayName = "SetHomePresenceAsync sends the spec-aligned presence lock command")]
        public async Task SetHomePresenceAsync_SendsSpecAlignedPresenceLockCommand()
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

            var service = new TadoHomeService(mockHttp.Object);

            await service.SetHomePresenceAsync(homeId: 1, presence: "HOME", CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/presenceLock",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.Is<object?>(body => body != null && JsonSerializer.Serialize(body).Contains("\"homePresence\":\"HOME\""))),
                Times.Once);
        }

        [Fact(DisplayName = "SetHomePresenceAsync rejects invalid presence values")]
        public async Task SetHomePresenceAsync_RejectsInvalidPresenceValues()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoHomeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetHomePresenceAsync(homeId: 1, presence: "MAYBE", CancellationToken.None));

            Assert.Equal("presence", exception.ParamName);
            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "ResetHomePresenceAsync sends the spec-aligned presence lock delete command")]
        public async Task ResetHomePresenceAsync_SendsSpecAlignedPresenceLockDeleteCommand()
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

            var service = new TadoHomeService(mockHttp.Object);

            await service.ResetHomePresenceAsync(homeId: 1, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/presenceLock",
                    HttpMethod.Delete,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    null),
                Times.Once);
        }

        [Fact(DisplayName = "SetAwayRadiusInMetersAsync sends the spec-aligned away-radius command")]
        public async Task SetAwayRadiusInMetersAsync_SendsSpecAlignedAwayRadiusCommand()
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

            var service = new TadoHomeService(mockHttp.Object);

            await service.SetAwayRadiusInMetersAsync(homeId: 1, awayRadiusInMeters: 400, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/awayRadiusInMeters",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.Is<SetAwayRadiusInMetersRequest>(body => body.AwayRadiusInMeters == 400)),
                Times.Once);
        }

        [Fact(DisplayName = "SetAwayRadiusInMetersAsync rejects negative distances")]
        public async Task SetAwayRadiusInMetersAsync_RejectsNegativeDistances()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoHomeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                service.SetAwayRadiusInMetersAsync(homeId: 1, awayRadiusInMeters: -1, CancellationToken.None));

            Assert.Equal("awayRadiusInMeters", exception.ParamName);
            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "SetIncidentDetectionAsync sends the spec-aligned incident-detection command")]
        public async Task SetIncidentDetectionAsync_SendsSpecAlignedIncidentDetectionCommand()
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

            var service = new TadoHomeService(mockHttp.Object);

            await service.SetIncidentDetectionAsync(homeId: 1, enabled: true, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/incidentDetection",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.Is<SetIncidentDetectionRequest>(body => body.Enabled && JsonSerializer.Serialize(body).Contains("\"enabled\":true"))),
                Times.Once);
        }

        [Fact(DisplayName = "SetHomeDetailsAsync sends the spec-aligned home-details command")]
        public async Task SetHomeDetailsAsync_SendsSpecAlignedHomeDetailsCommand()
        {
            string? capturedJson = null;
            var homeDetails = new House
            {
                Name = "My Home",
                ContactDetails = new ContactDetails
                {
                    Name = "Jane Doe",
                    Email = "jane@example.com",
                    Phone = "+441234567890"
                },
                Address = new Address
                {
                    AddressLine1 = "1 Test Street",
                    AddressLine2 = null,
                    ZipCode = "SW1A 1AA",
                    City = "London",
                    State = null,
                    Country = "GBR"
                },
                Geolocation = new Geolocation
                {
                    Latitude = 51.501,
                    Longitude = -0.141
                }
            };

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
                    capturedJson = JsonSerializer.Serialize(body);
                })
                .ReturnsAsync(true);

            var service = new TadoHomeService(mockHttp.Object);

            await service.SetHomeDetailsAsync(homeId: 1, homeDetails, CancellationToken.None);

            Assert.NotNull(capturedJson);
            Assert.Contains("\"name\":\"My Home\"", capturedJson);
            Assert.Contains("\"contactDetails\":{", capturedJson);
            Assert.Contains("\"address\":{", capturedJson);
            Assert.Contains("\"geolocation\":{", capturedJson);
            Assert.Contains("\"addressLine1\":\"1 Test Street\"", capturedJson);
            Assert.Contains("\"zipCode\":\"SW1A 1AA\"", capturedJson);
            Assert.Contains("\"latitude\":51.501", capturedJson);
            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/details",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.IsAny<SetHomeDetailsRequest>()),
                Times.Once);
        }

        [Fact(DisplayName = "SetHomeDetailsAsync rejects missing home names")]
        public async Task SetHomeDetailsAsync_RejectsMissingHomeNames()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoHomeService(mockHttp.Object);
            var homeDetails = new House
            {
                Name = string.Empty,
                ContactDetails = new ContactDetails(),
                Address = new Address(),
                Geolocation = new Geolocation()
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetHomeDetailsAsync(homeId: 1, homeDetails, CancellationToken.None));

            Assert.Equal("homeDetails", exception.ParamName);
            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        [Fact(DisplayName = "SetFlowTemperatureOptimisationAsync sends the spec-aligned flow-temperature command")]
        public async Task SetFlowTemperatureOptimisationAsync_SendsSpecAlignedFlowTemperatureCommand()
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

            var service = new TadoHomeService(mockHttp.Object);

            await service.SetFlowTemperatureOptimisationAsync(homeId: 1, maxFlowTemperature: 55, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/flowTemperatureOptimization",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.Is<SetFlowTemperatureOptimisationRequest>(body => body.MaxFlowTemperature == 55 && JsonSerializer.Serialize(body).Contains("\"maxFlowTemperature\":55"))),
                Times.Once);
        }
    }
}