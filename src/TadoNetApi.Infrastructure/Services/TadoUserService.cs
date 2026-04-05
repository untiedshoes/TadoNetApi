using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IUserService using the Tado API.
/// </summary>
public class TadoUserService : IUserService
{
    private readonly ITadoHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of <see cref="TadoUserService"/>.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the Tado API.</param>
    public TadoUserService(ITadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #region Data Retrieval

    /// <summary>
    /// Retrieves information about the current authenticated user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current user, or <see langword="null"/> when no payload is returned.</returns>
    public async Task<User?> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoUserResponse>("me", cancellationToken);
        return dto == null ? null : UserMapper.ToDomain(dto);
    }

    #endregion

}