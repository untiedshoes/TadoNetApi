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
    }
}