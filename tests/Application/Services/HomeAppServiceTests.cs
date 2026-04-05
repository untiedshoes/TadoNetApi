using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services
{
    public class HomeAppServiceTests
    {
        [Fact]
        public async Task GetHomeAsync_ReturnsHome()
        {
            // Arrange
            var expectedHome = new House
            {
                Id = 1,
                Name = "My Home",
                DateTimeZone = "Europe/London",
                TemperatureUnit = "C",
                InstallationCompleted = true,
                ChristmasModeEnabled = false,
                IncidentDetection = new IncidentDetection
                {
                    Enabled = true,
                    Supported = true
                },
                ContactDetails = new ContactDetails(),
                Address = new Address(),
                Geolocation = new Geolocation()
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHomeAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedHome);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var home = await service.GetHomeAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(home);
            Assert.Equal(1, home?.Id);
            Assert.Equal("My Home", home?.Name);
            Assert.True(home?.IncidentDetection?.Enabled);
        }

        [Fact]
        public async Task GetHomeStateAsync_ReturnsState()
        {
            // Arrange
            var expectedState = new HomeState
            {
                Presence = "HOME"
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHomeStateAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedState);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var state = await service.GetHomeStateAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(state);
            Assert.Equal("HOME", state?.Presence);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new() { Id = "user-1", Name = "Alice Example", Email = "alice@example.com" },
                new() { Id = "user-2", Name = "Bob Example", Email = "bob@example.com" }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetUsersAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUsers);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var users = await service.GetUsersAsync(1, CancellationToken.None);

            // Assert
            Assert.Equal(2, users.Count);
            Assert.Equal("Alice Example", users[0].Name);
            Assert.Equal("Bob Example", users[1].Name);
        }

        [Fact]
        public async Task GetAirComfortAsync_ReturnsAirComfort()
        {
            // Arrange
            var expectedAirComfort = new AirComfort
            {
                Freshness = new AirComfortFreshness
                {
                    Value = "FRESH"
                },
                AcPoweredOn = false,
                Comfort =
                [
                    new AirComfortComfort
                    {
                        RoomId = 1,
                        TemperatureLevel = "COMFY",
                        HumidityLevel = "COMFY",
                        Coordinate = new AirComfortCoordinate
                        {
                            Radial = 0.22,
                            Angular = 76
                        }
                    }
                ]
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetAirComfortAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAirComfort);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var airComfort = await service.GetAirComfortAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(airComfort);
            Assert.Equal("FRESH", airComfort.Freshness?.Value);
            Assert.False(airComfort.AcPoweredOn);
            Assert.Single(airComfort.Comfort!);
            Assert.Equal(1, airComfort.Comfort![0].RoomId);
            Assert.Equal(76, airComfort.Comfort[0].Coordinate?.Angular);
        }

        [Fact]
        public async Task GetIncidentDetectionAsync_ReturnsIncidentDetection()
        {
            // Arrange
            var expectedIncidentDetection = new IncidentDetection
            {
                Enabled = true,
                Supported = true
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetIncidentDetectionAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedIncidentDetection);

            var service = new HomeAppService(mockHomeService.Object);

            // Act
            var incidentDetection = await service.GetIncidentDetectionAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(incidentDetection);
            Assert.True(incidentDetection.Enabled);
            Assert.True(incidentDetection.Supported);
        }

        [Fact]
        public async Task GetHeatingCircuitsAsync_ReturnsHeatingCircuits()
        {
            var expectedCircuits = new List<HeatingCircuit>
            {
                new() { Number = 1, DriverSerialNo = "BR3209250550", DriverShortSerialNo = "BR3209250550" }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHeatingCircuitsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCircuits);

            var service = new HomeAppService(mockHomeService.Object);

            var circuits = await service.GetHeatingCircuitsAsync(1, CancellationToken.None);

            Assert.Single(circuits);
            Assert.Equal(1, circuits[0].Number);
            Assert.Equal("BR3209250550", circuits[0].DriverSerialNo);
        }

        [Fact]
        public async Task GetHeatingSystemAsync_ReturnsHeatingSystem()
        {
            var expectedHeatingSystem = new HeatingSystem
            {
                Boiler = new Boiler
                {
                    Present = true,
                    Id = 2699,
                    Found = true
                },
                UnderfloorHeating = new UnderfloorHeating
                {
                    Present = false
                }
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetHeatingSystemAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedHeatingSystem);

            var service = new HomeAppService(mockHomeService.Object);

            var heatingSystem = await service.GetHeatingSystemAsync(1, CancellationToken.None);

            Assert.NotNull(heatingSystem);
            Assert.True(heatingSystem.Boiler?.Present);
            Assert.Equal(2699, heatingSystem.Boiler?.Id);
            Assert.False(heatingSystem.UnderfloorHeating?.Present);
        }

        [Fact]
        public async Task GetFlowTemperatureOptimisationAsync_ReturnsFlowTemperatureOptimisation()
        {
            var expectedFlowTemperatureOptimisation = new FlowTemperatureOptimisation
            {
                HasMultipleBoilerControlDevices = false,
                MaxFlowTemperature = 50,
                MaxFlowTemperatureConstraints = new FlowTemperatureOptimisationConstraints
                {
                    Min = 30,
                    Max = 80
                },
                AutoAdaptation = new FlowTemperatureOptimisationAutoAdaptation
                {
                    Enabled = true,
                    MaxFlowTemperature = 45
                },
                OpenThermDeviceSerialNumber = "BR1234567890"
            };

            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.GetFlowTemperatureOptimisationAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFlowTemperatureOptimisation);

            var service = new HomeAppService(mockHomeService.Object);

            var flowTemperatureOptimisation = await service.GetFlowTemperatureOptimisationAsync(1, CancellationToken.None);

            Assert.NotNull(flowTemperatureOptimisation);
            Assert.False(flowTemperatureOptimisation.HasMultipleBoilerControlDevices);
            Assert.Equal(50, flowTemperatureOptimisation.MaxFlowTemperature);
            Assert.Equal(30, flowTemperatureOptimisation.MaxFlowTemperatureConstraints?.Min);
            Assert.Equal(80, flowTemperatureOptimisation.MaxFlowTemperatureConstraints?.Max);
            Assert.True(flowTemperatureOptimisation.AutoAdaptation?.Enabled);
            Assert.Equal("BR1234567890", flowTemperatureOptimisation.OpenThermDeviceSerialNumber);
        }

        [Fact]
        public async Task SetHomePresenceAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.SetHomePresenceAsync(1, "HOME", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.SetHomePresenceAsync(1, "HOME", CancellationToken.None);

            mockHomeService.Verify(s => s.SetHomePresenceAsync(1, "HOME", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ResetHomePresenceAsync_PassesThroughToDomainService()
        {
            var mockHomeService = new Mock<IHomeService>();
            mockHomeService.Setup(s => s.ResetHomePresenceAsync(1, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new HomeAppService(mockHomeService.Object);

            await service.ResetHomePresenceAsync(1, CancellationToken.None);

            mockHomeService.Verify(s => s.ResetHomePresenceAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}