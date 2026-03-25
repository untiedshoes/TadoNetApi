using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    public class ZoneServiceTests
    {
        [Fact]
        public async Task GetZonesAsync_ReturnsZones()
        {
            // Arrange
            var tadoZones = new List<TadoZoneResponse>
            {
                new TadoZoneResponse
                {
                    Id = 1,
                    Name = "Living Room",
                    Type = "HEATING",
                    Setting = new TadoZoneSetting { Temperature = 21.0, Mode = "HEATING" },
                    State = new TadoZoneState { Temperature = 20.0, Humidity = 50, IsHeating = true }
                },
                new TadoZoneResponse
                {
                    Id = 2,
                    Name = "Bedroom",
                    Type = "HEATING",
                    Setting = new TadoZoneSetting { Temperature = 19.0, Mode = "HEATING" },
                    State = new TadoZoneState { Temperature = 18.5, Humidity = 45, IsHeating = false }
                }
            };

            // Mock the HTTP client to return our DTOs
            var mockHttp = MockTadoHttpClient.CreateGet(tadoZones);

            // Inject mock into the service (using the interface)
            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var zones = await service.GetZonesAsync(homeId: 1);

            // Assert - verify Domain mapping
            Assert.Equal(2, zones.Count);

            // First zone
            var livingRoom = zones[0];
            Assert.Equal(1, livingRoom.Id);
            Assert.Equal("Living Room", livingRoom.Name);
            Assert.Equal(21.0, livingRoom.TargetTemperature);
            Assert.Equal(20.0, livingRoom.CurrentTemperature);
            Assert.Equal(50, livingRoom.Humidity);
            Assert.True(livingRoom.IsHeating);

            // Second zone
            var bedroom = zones[1];
            Assert.Equal(2, bedroom.Id);
            Assert.Equal("Bedroom", bedroom.Name);
            Assert.Equal(19.0, bedroom.TargetTemperature);
            Assert.Equal(18.5, bedroom.CurrentTemperature);
            Assert.Equal(45, bedroom.Humidity);
            Assert.False(bedroom.IsHeating);
        }
    }
}