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
    Task<User?> GetMeAsync(CancellationToken cancellationToken = default);
}