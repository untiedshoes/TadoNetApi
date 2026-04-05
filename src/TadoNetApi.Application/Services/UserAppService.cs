using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services;

/// <summary>
/// Application service for managing user information.
/// </summary>
public class UserAppService
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of UserAppService.
    /// </summary>
    /// <param name="userService">The domain user service to use.</param>
    public UserAppService(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves information about the current user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current user, or <see langword="null"/> when no payload is returned.</returns>
    public Task<User?> GetMeAsync(CancellationToken cancellationToken) => _userService.GetMeAsync(cancellationToken);
}