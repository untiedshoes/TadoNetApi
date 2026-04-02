using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    public class HomeServiceTests
    {
        [Fact]
        public async Task GetHomeAsync_ReturnsHome()
        {
            // Arrange
            var tadoHouse = new TadoHouseResponse
            {
                Id = 1,
                Name = "My Home",
                DateTimeZone = "Europe/London",
                DateCreated = DateTime.UtcNow,
                TemperatureUnit = "C",
                InstallationCompleted = true,
                Partner = null,
                SimpleSmartScheduleEnabled = false,
                AwayRadiusInMeters = 0,
                License = "MIT",
                ChristmasModeEnabled = false,
                ContactDetails = new TadoContactDetailsResponse(),
                Address = new TadoAddressResponse(),
                Geolocation = new TadoGeolocationResponse()
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoHouse);
            var service = new TadoHomeService(mockHttp.Object);

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
            var tadoState = new TadoHomeStateResponse
            {
                Presence = "HOME"
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoState);
            var service = new TadoHomeService(mockHttp.Object);

            // Act
            var state = await service.GetHomeStateAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(state);
            Assert.Equal("HOME", state?.Presence);
        }
    }
}