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
    /// <param name="homeId">The ID of the home to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested home, or <see langword="null"/> when no payload is returned.</returns>
    public Task<House?> GetHomeAsync(int homeId, CancellationToken cancellationToken) => 
        _homeService.GetHomeAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the state of a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current home state, or <see langword="null"/> when no payload is returned.</returns>
    public Task<HomeState?> GetHomeStateAsync(int homeId, CancellationToken cancellationToken) => 
        _homeService.GetHomeStateAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the users associated with a home.
    /// </summary>
    /// <param name="homeId">The ID of the home whose users should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of users associated with the home.</returns>
    public Task<IReadOnlyList<User>> GetUsersAsync(int homeId, CancellationToken cancellationToken) => 
        _homeService.GetUsersAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the air comfort indicators for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current air comfort indicators.</returns>
    public Task<AirComfort> GetAirComfortAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetAirComfortAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the installations configured for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of installations associated with the home.</returns>
    public Task<IReadOnlyList<Installation>> GetInstallationsAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetInstallationsAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves a specific installation for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="installationId">The ID of the installation to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested installation, or <see langword="null"/> when no payload is returned.</returns>
    public Task<Installation?> GetInstallationAsync(int homeId, int installationId, CancellationToken cancellationToken) =>
        _homeService.GetInstallationAsync(homeId, installationId, cancellationToken);

    /// <summary>
    /// Retrieves the pending invitations for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of pending invitations associated with the home.</returns>
    public Task<IReadOnlyList<Invitation>> GetInvitationsAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetInvitationsAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the incident detection settings for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The incident detection configuration.</returns>
    public Task<IncidentDetection> GetIncidentDetectionAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetIncidentDetectionAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the heating circuits configured for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of configured heating circuits.</returns>
    public Task<IReadOnlyList<HeatingCircuit>> GetHeatingCircuitsAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetHeatingCircuitsAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the heating system configuration for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The configured heating system.</returns>
    public Task<HeatingSystem> GetHeatingSystemAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetHeatingSystemAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves the flow-temperature optimisation settings for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The flow-temperature optimisation settings.</returns>
    public Task<FlowTemperatureOptimisation> GetFlowTemperatureOptimisationAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.GetFlowTemperatureOptimisationAsync(homeId, cancellationToken);

    /// <summary>
    /// Sets the presence state of a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="presence">The presence value to apply, such as HOME or AWAY.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken) =>
        _homeService.SetHomePresenceAsync(homeId, presence, cancellationToken);

    /// <summary>
    /// Resets the manually set presence state of a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task ResetHomePresenceAsync(int homeId, CancellationToken cancellationToken) =>
        _homeService.ResetHomePresenceAsync(homeId, cancellationToken);

    /// <summary>
    /// Sets the geo-tracking away radius for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="awayRadiusInMeters">The distance in meters at which a device is considered away from home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task SetAwayRadiusInMetersAsync(int homeId, double awayRadiusInMeters, CancellationToken cancellationToken) =>
        _homeService.SetAwayRadiusInMetersAsync(homeId, awayRadiusInMeters, cancellationToken);

    /// <summary>
    /// Enables or disables incident detection for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="enabled">Whether incident detection should be enabled.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task SetIncidentDetectionAsync(int homeId, bool enabled, CancellationToken cancellationToken) =>
        _homeService.SetIncidentDetectionAsync(homeId, enabled, cancellationToken);

    /// <summary>
    /// Updates the writable home details for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="homeDetails">The complete writable home details payload.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task SetHomeDetailsAsync(int homeId, House homeDetails, CancellationToken cancellationToken) =>
        _homeService.SetHomeDetailsAsync(homeId, homeDetails, cancellationToken);

    /// <summary>
    /// Updates the maximum flow temperature for a home's boiler optimisation.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="maxFlowTemperature">The maximum flow temperature to apply.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task SetFlowTemperatureOptimisationAsync(int homeId, int maxFlowTemperature, CancellationToken cancellationToken) =>
        _homeService.SetFlowTemperatureOptimisationAsync(homeId, maxFlowTemperature, cancellationToken);

    /// <summary>
    /// Sends an invitation to join a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="email">The email address to invite.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The created invitation.</returns>
    public Task<Invitation> SendInvitationAsync(int homeId, string email, CancellationToken cancellationToken) =>
        _homeService.SendInvitationAsync(homeId, email, cancellationToken);

    /// <summary>
    /// Revokes a pending invitation.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="invitationToken">The unique invitation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task DeleteInvitationAsync(int homeId, string invitationToken, CancellationToken cancellationToken) =>
        _homeService.DeleteInvitationAsync(homeId, invitationToken, cancellationToken);

    /// <summary>
    /// Resends a pending invitation.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="invitationToken">The unique invitation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public Task ResendInvitationAsync(int homeId, string invitationToken, CancellationToken cancellationToken) =>
        _homeService.ResendInvitationAsync(homeId, invitationToken, cancellationToken);
}