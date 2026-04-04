using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoZoneServiceReadTests
    {
        [Fact(DisplayName = "GetZoneControlAsync returns mapped zone control")]
        public async Task GetZoneControlAsync_ReturnsMappedZoneControl()
        {
            // Arrange
            var response = new TadoZoneControlResponse
            {
                Type = "HEATING",
                EarlyStartEnabled = true,
                HeatingCircuit = 1,
                Duties = new TadoZoneControlDutiesResponse
                {
                    Type = "HEATING",
                    Leader = new TadoDeviceResponse
                    {
                        DeviceType = "SU02",
                        SerialNo = "SU1234567890",
                        ShortSerialNo = "SU1234567890"
                    },
                    Drivers =
                    [
                        new TadoDeviceResponse
                        {
                            DeviceType = "VA02",
                            SerialNo = "VA1234567890",
                            ShortSerialNo = "VA1234567890"
                        }
                    ]
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var control = await service.GetZoneControlAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            // Assert
            Assert.Equal("HEATING", control.Type);
            Assert.True(control.EarlyStartEnabled);
            Assert.Equal(1, control.HeatingCircuit);
            Assert.Equal("SU1234567890", control.Duties?.Leader?.SerialNo);
            Assert.Single(control.Duties?.Drivers!);
            Assert.Equal("VA1234567890", control.Duties?.Drivers?[0].SerialNo);
        }

        [Fact(DisplayName = "GetDefaultZoneOverlayAsync returns mapped default overlay")]
        public async Task GetDefaultZoneOverlayAsync_ReturnsMappedDefaultOverlay()
        {
            // Arrange
            var response = new TadoDefaultZoneOverlayResponse
            {
                TerminationCondition = new TadoTerminationResponse
                {
                    CurrentType = DurationModes.Timer,
                    DurationInSeconds = 900
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var defaultOverlay = await service.GetDefaultZoneOverlayAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            // Assert
            Assert.NotNull(defaultOverlay);
            Assert.Equal("Timer", defaultOverlay.TerminationCondition?.Type);
            Assert.Equal(900, defaultOverlay.TerminationCondition?.DurationInSeconds);
        }
    }
}