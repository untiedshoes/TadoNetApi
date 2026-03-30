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
                new TadoDeviceResponse
                {
                    SerialNo = "123456789",
                    ShortSerialNo = "123456",
                    DeviceType = "THERMOSTAT",
                    CurrentFwVersion = "1.0.0"
                },
                new TadoDeviceResponse
                {
                    SerialNo = "987654321",
                    ShortSerialNo = "987654",
                    DeviceType = "RADIATOR",
                    CurrentFwVersion = "1.0.0"
                }
            };

            // Simulate 2 transient failures before returning devices
            var mockHttp = MockTadoHttpClient.CreateGet(tadoDevices, transientFailures: 0, transientException: new HttpRequestException("Transient error"));
            var mockAuth = MockTadoAuthService.CreateAuthenticated();

            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var devices = await service.GetDevicesAsync(homeId: 1, zoneId: 1, cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(devices);
            Assert.Equal(2, devices.Count);
            Assert.Equal("123456789", devices[0].SerialNo);
            Assert.Equal("THERMOSTAT", devices[0].DeviceType);
            Assert.Equal("987654321", devices[1].SerialNo);
            Assert.Equal("RADIATOR", devices[1].DeviceType);    
        }
    }
}