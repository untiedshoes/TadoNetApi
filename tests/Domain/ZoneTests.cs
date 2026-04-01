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
                CurrentType = "HEATING"
            };

            // Act
            // Just initialization

            // Assert
            Assert.Equal(1, zone.Id);
            Assert.Equal("Living Room", zone.Name);
            Assert.Equal("HEATING", zone.CurrentType);
        }
    }
}