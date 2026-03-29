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
    public class HomeServiceTests
    {
        [Fact]
        public async Task GetHomesAsync_ReturnsHomes()
        {
            // Arrange
            var tadoHomes = new List<TadoHomeResponse>
            {
                new TadoHomeResponse
                {
                    Id = 1,
                    Name = "My Home",
                    Timezone = "Europe/London"
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoHomes);
            var mockAuth = MockTadoAuthService.CreateAuthenticated();

            var service = new TadoHomeService(mockHttp.Object);

            // Act
            var homes = await service.GetHomesAsync(CancellationToken.None);

            // Assert
            Assert.Single(homes);
            Assert.Equal(1, homes[0].Id);
            Assert.Equal("My Home", homes[0].Name);
            Assert.Equal("Europe/London", homes[0].Timezone);
        }
    }
}