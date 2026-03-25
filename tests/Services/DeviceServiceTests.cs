using System;
using System.Collections.Generic;
using System.Net.Http;
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
    public class DeviceServiceTests
    {
        [Fact]
        public async Task GetDevicesAsync_TransientFailures_RetriesAndReturnsDevices()
        {
            // Arrange
            var tadoDevices = new List<TadoDeviceResponse>
            {
                new TadoDeviceResponse { Id = 1, Name = "Thermostat", ChildLock = false },
                new TadoDeviceResponse { Id = 2, Name = "Radiator", ChildLock = true }
            };

            // Simulate 2 transient failures before returning devices
            var mockHttp = MockTadoHttpClient.CreateGet(tadoDevices, transientFailures: 0, transientException: new HttpRequestException("Transient error"));
            var mockAuth = MockTadoAuthService.CreateAuthenticated();

            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var devices = await service.GetDevicesAsync(homeId: 1, zoneId: 1, cancellationToken: CancellationToken.None);

            // Assert
            Assert.Equal(2, devices.Count);
            Assert.Equal("Thermostat", devices[0].Name);
            Assert.False(devices[0].ChildLock);
            Assert.Equal("Radiator", devices[1].Name);
            Assert.True(devices[1].ChildLock);
        }
    }
}