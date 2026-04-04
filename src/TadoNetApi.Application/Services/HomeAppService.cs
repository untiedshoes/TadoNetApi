using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services;

/// <summary>
/// Application service for managing homes.
/// Orchestrates domain operations through IHomeService.
/// </summary>
public class HomeAppService
{
    private readonly IHomeService _homeService;

    /// <summary>
    /// Initializes a new instance of HomeAppService.
    /// </summary>
    /// <param name="homeService">The domain home service to use.</param>
    public HomeAppService(IHomeService homeService)
    {
        _homeService = homeService;
    }

    /// <summary>
    /// Retrieves a specific home by ID.
    /// </summary>
    public Task<House?> GetHomeAsync(int homeId, CancellationToken cancellationToken) => 
        _homeService.GetHomeAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the state of a home.
    /// </summary>
    public Task<HomeState?> GetHomeStateAsync(int homeId, CancellationToken cancellationToken) => 
        _homeService.GetHomeStateAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the users associated with a home.
    /// </summary>
    public Task<IReadOnlyList<User>> GetUsersAsync(int homeId, CancellationToken cancellationToken) => 
        _homeService.GetUsersAsync(homeId, cancellationToken);

    /// <summary>
    /// Sets the presence state of a home.
    /// </summary>
    public Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken) =>
        _homeService.SetHomePresenceAsync(homeId, presence, cancellationToken);
}