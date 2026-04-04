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
    /// Retrieves the air comfort indicators for a home.
    /// </summary>
    public Task<AirComfort> GetAirComfortAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetAirComfortAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the incident detection settings for a home.
    /// </summary>
    public Task<IncidentDetection> GetIncidentDetectionAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetIncidentDetectionAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the heating circuits configured for a home.
    /// </summary>
    public Task<IReadOnlyList<HeatingCircuit>> GetHeatingCircuitsAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetHeatingCircuitsAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the heating system configuration for a home.
    /// </summary>
    public Task<HeatingSystem> GetHeatingSystemAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetHeatingSystemAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the flow-temperature optimisation settings for a home.
    /// </summary>
    public Task<FlowTemperatureOptimisation> GetFlowTemperatureOptimisationAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetFlowTemperatureOptimisationAsync(homeId, cancellationToken);

    /// <summary>
    /// Sets the presence state of a home.
    /// </summary>
    public Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken) =>
        _homeService.SetHomePresenceAsync(homeId, presence, cancellationToken);
}