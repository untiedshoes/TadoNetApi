using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services;

/// <summary>
/// Unit tests for <see cref="TadoUserService"/>.
/// </summary>
public class TadoUserServiceTests
{
    /// <summary>
    /// GetMeAsync returns mapped user when API provides payload.
    /// </summary>
    [Fact(DisplayName = "GetMeAsync returns mapped user when API provides payload")]
    public async Task GetMeAsync_ReturnsMappedUser_WhenApiProvidesPayload()
    {
        var mockHttp = new Mock<ITadoHttpClient>();
        mockHttp
            .Setup(c => c.GetAsync<TadoUserResponse>("me", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TadoUserResponse
            {
                Id = "user-1",
                Name = "Alice",
                Email = "alice@example.com",
                Username = "alice"
            });

        var service = new TadoUserService(mockHttp.Object);

        var result = await service.GetMeAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("user-1", result!.Id);
        Assert.Equal("Alice", result.Name);
    }

    /// <summary>
    /// GetMeAsync returns null when API payload is null.
    /// </summary>
    [Fact(DisplayName = "GetMeAsync returns null when API payload is null")]
    public async Task GetMeAsync_ReturnsNull_WhenApiPayloadIsNull()
    {
        var mockHttp = new Mock<ITadoHttpClient>();
        mockHttp
            .Setup(c => c.GetAsync<TadoUserResponse>("me", It.IsAny<CancellationToken>()))
            .ReturnsAsync((TadoUserResponse?)null);

        var service = new TadoUserService(mockHttp.Object);

        var result = await service.GetMeAsync(CancellationToken.None);

        Assert.Null(result);
    }
}