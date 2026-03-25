using TadoNetApi.Domain.Entities;
using Xunit;

namespace TadoNetApi.Tests.Domain
{
    public class ZoneTests
    {
        [Fact]
        public void Can_Create_Zone_With_Properties()
        {
            // Arrange
            var zone = new Zone
            {
                Id = 1,
                Name = "Living Room",
                CurrentTemperature = 21.5,
                TargetTemperature = 22,
                IsHeating = true
            };

            // Act
            // Just initialization

            // Assert
            Assert.Equal(1, zone.Id);
            Assert.Equal("Living Room", zone.Name);
            Assert.Equal(21.5, zone.CurrentTemperature);
            Assert.Equal(22, zone.TargetTemperature);
            Assert.True(zone.IsHeating);
        }
    }
}