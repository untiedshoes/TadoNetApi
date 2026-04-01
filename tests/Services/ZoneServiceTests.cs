using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="TadoZoneService"/>.
    /// Covers zone retrieval, zone state mapping, zone summary mapping, and error handling.
    /// </summary>
    public class ZoneServiceTests
    {
        /// <summary>
        /// Creates a <see cref="TadoZoneService"/> with a mocked HTTP response.
        /// </summary>
        /// <typeparam name="T">Type of DTO to return from the mock.</typeparam>
        /// <param name="response">The response object to return.</param>
        /// <returns>A TadoZoneService instance with mocked ITadoHttpClient.</returns>
        private TadoZoneService CreateServiceWithResponse<T>(T response)
        {
            var mockHttp = MockTadoHttpClient.CreateGet(response);
            return new TadoZoneService(mockHttp.Object);
        }

        /// <summary>
        /// Tests that <see cref="TadoZoneService.GetZonesAsync"/> returns correctly mapped zones.
        /// </summary>
        [Fact]
        public async Task GetZonesAsync_ReturnsMappedZones()
        {
            // Arrange
            var zonesDto = new List<TadoZoneResponse>
            {
                new() { Id = 1, Name = "Living Room", CurrentType = "HEATING" },
                new() { Id = 2, Name = "Bedroom", CurrentType = "HEATING" }
            };

            var service = CreateServiceWithResponse(zonesDto);

            // Act
            var zones = await service.GetZonesAsync(1);

            // Assert
            Assert.Equal(2, zones.Count);
            Assert.Equal("Living Room", zones[0].Name);
            Assert.Equal("Bedroom", zones[1].Name);
        }

        /// <summary>
        /// Tests that <see cref="TadoZoneService.GetZoneAsync"/> returns a correctly mapped zone.
        /// </summary>
        [Fact]
        public async Task GetZoneAsync_ReturnsMappedZone()
        {
            // Arrange
            var dto = new TadoZoneResponse
            {
                Id = 1,
                Name = "Living Room",
                CurrentType = "HEATING"
            };

            var service = CreateServiceWithResponse(dto);

            // Act
            var zone = await service.GetZoneAsync(1, 1);

            // Assert
            Assert.NotNull(zone);
            Assert.Equal(1, zone.Id);
            Assert.Equal("Living Room", zone.Name);
        }

        /// <summary>
        /// Tests that <see cref="TadoZoneService.GetZoneStateAsync"/> returns a correctly mapped state.
        /// </summary>
        [Fact]
        public async Task GetZoneStateAsync_ReturnsMappedState()
        {
            // Arrange
            var stateDto = new TadoStateResponse
            {
                TadoMode = "HOME",
                GeolocationOverride = true,
                Setting = new TadoSettingResponse
                {
                    Power = PowerStates.On,
                    Temperature = new TadoTemperatureResponse { Celsius = 22 }
                }
            };

            var service = CreateServiceWithResponse(stateDto);

            // Act
            var state = await service.GetZoneStateAsync(1, 1);

            // Assert
            Assert.NotNull(state);
            Assert.Equal("HOME", state.TadoMode);
            Assert.True(state.GeolocationOverride);
            Assert.Equal(PowerStates.On, state.Setting?.Power);
            Assert.Equal(22, state.Setting?.Temperature?.Celsius);
        }

        /// <summary>
        /// Tests that <see cref="TadoZoneService.GetZoneSummaryAsync"/> returns a correctly mapped zone summary.
        /// </summary>
        [Fact]
        public async Task GetZoneSummaryAsync_ReturnsMappedSummary()
        {
            // Arrange
            var summaryDto = new TadoZoneSummaryResponse
            {
                Setting = new TadoSettingResponse
                {
                    Power = PowerStates.On,
                    Temperature = new TadoTemperatureResponse { Celsius = 21.5 }
                },
                Termination = new TadoTerminationResponse
                {
                    CurrentType = DurationModes.Timer,
                    DurationInSeconds = 3600
                }
            };

            var service = CreateServiceWithResponse(summaryDto);

            // Act
            var summary = await service.GetZoneSummaryAsync(1, 1);

            // Assert
            Assert.NotNull(summary);
            Assert.Equal(PowerStates.On, summary.Setting?.Power);
            Assert.Equal(21.5, summary.Setting?.Temperature?.Celsius);
            Assert.Equal(DurationModes.Timer.ToString(), summary.Termination?.Type);
            Assert.Equal(3600, summary.Termination?.DurationInSeconds);
        }
    }
}