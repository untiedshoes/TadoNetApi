using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    }
}