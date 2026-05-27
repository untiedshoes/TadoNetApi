using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services;

/// <summary>
/// Unit tests for <see cref="UserAppService"/>.
/// </summary>
public class UserAppServiceTests
{
    /// <summary>
    /// GetMeAsync forwards the call and returns the user payload from the domain service.
    /// </summary>
    [Fact(DisplayName = "GetMeAsync forwards call and returns user payload")]
    public async Task GetMeAsync_ForwardsCallAndReturnsUserPayload()
    {
        var cancellationToken = new CancellationTokenSource().Token;
        var expected = new User
        {
            Id = "user-1",
            Name = "Test User",
            Email = "test@example.com"
        };

        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(s => s.GetMeAsync(cancellationToken))
            .ReturnsAsync(expected);

        var service = new UserAppService(mockUserService.Object);

        var result = await service.GetMeAsync(cancellationToken);

        Assert.Same(expected, result);
        mockUserService.Verify(s => s.GetMeAsync(cancellationToken), Times.Once);
    }

    /// <summary>
    /// GetMeAsync preserves a null result from the domain service.
    /// </summary>
    [Fact(DisplayName = "GetMeAsync returns null when domain service returns null")]
    public async Task GetMeAsync_ReturnsNull_WhenDomainServiceReturnsNull()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(s => s.GetMeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var service = new UserAppService(mockUserService.Object);

        var result = await service.GetMeAsync(CancellationToken.None);

        Assert.Null(result);
    }
}