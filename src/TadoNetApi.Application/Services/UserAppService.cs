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
    public Task<User?> GetMeAsync(CancellationToken cancellationToken) => _userService.GetMeAsync(cancellationToken);
}