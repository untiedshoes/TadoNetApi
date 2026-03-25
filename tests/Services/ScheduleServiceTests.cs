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
    public class ScheduleServiceTests
    {
        [Fact]
        public async Task GetZoneScheduleAsync_ReturnsSchedules()
        {
            // Arrange
            var now = DateTime.UtcNow.Date;

            var tadoSchedules = new List<TadoScheduleResponse>
            {
                new TadoScheduleResponse
                {
                    Name = "Morning",
                    TargetTemperature = 20.0,
                    IsActive = true,
                    StartTime = now.AddHours(6).ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndTime   = now.AddHours(9).ToString("yyyy-MM-ddTHH:mm:ss")
                },
                new TadoScheduleResponse
                {
                    Name = "Evening",
                    TargetTemperature = 18.0,
                    IsActive = false,
                    StartTime = now.AddHours(18).ToString("yyyy-MM-ddTHH:mm:ss"),
                    EndTime   = now.AddHours(22).ToString("yyyy-MM-ddTHH:mm:ss")
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoSchedules);
            var mockAuth = MockTadoAuthService.CreateAuthenticated();

            var service = new TadoScheduleService(mockHttp.Object);

            // Act
            var schedules = await service.GetZoneScheduleAsync(homeId: 1, zoneId: 1, cancellationToken: CancellationToken.None);

            // Assert
            Assert.Equal(2, schedules.Count);
            Assert.Equal("Morning", schedules[0].Name);
            Assert.True(schedules[0].IsActive);
            Assert.Equal("Evening", schedules[1].Name);
            Assert.False(schedules[1].IsActive);
        }
    }
}