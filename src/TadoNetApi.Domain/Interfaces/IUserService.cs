namespace TadoNetApi.Domain.Interfaces;

using TadoNetApi.Domain.Entities;

/// <summary>
/// Service interface for retrieving user information.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves information about the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current user, or <see langword="null"/> when no payload is returned.</returns>
    Task<User?> GetMeAsync(CancellationToken cancellationToken = default);
}