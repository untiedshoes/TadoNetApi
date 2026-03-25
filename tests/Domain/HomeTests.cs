using System;
using TadoNetApi.Domain.Entities;
using Xunit;

namespace TadoNetApi.Tests.Domain
{
    public class HomeTests
    {
        [Fact]
        public void Can_Create_Home_With_Properties()
        {
            // Arrange
            var home = new Home
            {
                Id = 1,
                Name = "Test Home",
                ShortName = "TH",
                CountryCode = "GB",
                IsActive = true
            };

            // Act
            // No complex logic, just entity initialization

            // Assert
            Assert.Equal(1, home.Id);
            Assert.Equal("Test Home", home.Name);
            Assert.Equal("TH", home.ShortName);
            Assert.Equal("GB", home.CountryCode);
            Assert.True(home.IsActive);
        }
    }
}