using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    public class TadoHomeServiceTests
    {
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
    }
}