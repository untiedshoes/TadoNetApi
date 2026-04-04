using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    public class HomeServiceTests
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
    }
}