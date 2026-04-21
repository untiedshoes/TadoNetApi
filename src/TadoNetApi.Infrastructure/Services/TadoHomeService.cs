using System.Net.Http;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Infrastructure.Validation;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IHomeService using the Tado API.
/// </summary>
public class TadoHomeService : IHomeService
{
    private readonly ITadoHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of <see cref="TadoHomeService"/>.
    /// </summary>
    /// <param name="httpClient">The HTTP client used to communicate with the Tado API.</param>
    public TadoHomeService(ITadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #region Data Retrieval

    /// <summary>
    /// Retrieves a specific home by its unique identifier.
    /// </summary>
    /// <param name="homeId">The ID of the home to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested home, or <see langword="null"/> when no payload is returned.</returns>
    public async Task<House?> GetHomeAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var dto = await _httpClient.GetAsync<TadoHouseResponse>($"homes/{homeId}", cancellationToken);
            return dto == null ? null : HouseMapper.ToDomain(dto);
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve home: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the state of a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current home state, or <see langword="null"/> when no payload is returned.</returns>
    public async Task<HomeState?> GetHomeStateAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var dto = await _httpClient.GetAsync<TadoHomeStateResponse>($"homes/{homeId}/state", cancellationToken);
            return dto == null ? null : dto.ToDomain();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve home state: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the users associated with a home.
    /// </summary>
    /// <param name="homeId">The ID of the home whose users should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of users associated with the home.</returns>
    public async Task<IReadOnlyList<User>> GetUsersAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<List<TadoUserResponse>>(
                $"homes/{homeId}/users",
                cancellationToken) ?? new List<TadoUserResponse>();

            return UserMapper.ToDomainList(response);
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve users: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the air comfort indicators for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current air comfort indicators.</returns>
    public async Task<AirComfort> GetAirComfortAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<TadoAirComfortResponse>(
                $"homes/{homeId}/airComfort",
                cancellationToken);

            if (response == null)
                throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                    $"Air comfort for home {homeId} not found.");

            return response.ToDomain();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve air comfort: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the installations configured for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of installations associated with the home.</returns>
    public async Task<IReadOnlyList<Installation>> GetInstallationsAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        var response = await _httpClient.GetAsync<List<TadoInstallationResponse>>(
            $"homes/{homeId}/installations",
            cancellationToken) ?? new List<TadoInstallationResponse>();

        return response.ToDomainList();
    }

    /// <summary>
    /// Retrieves a specific installation for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="installationId">The ID of the installation to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested installation, or <see langword="null"/> when no payload is returned.</returns>
    public async Task<Installation?> GetInstallationAsync(int homeId, int installationId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));
        Guard.PositiveId(installationId, nameof(installationId));

        var dto = await _httpClient.GetAsync<TadoInstallationResponse>(
            $"homes/{homeId}/installations/{installationId}",
            cancellationToken);

        return dto == null ? null : dto.ToDomain();
    }

    /// <summary>
    /// Retrieves the pending invitations for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of pending invitations associated with the home.</returns>
    public async Task<IReadOnlyList<Invitation>> GetInvitationsAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        var response = await _httpClient.GetAsync<List<TadoInvitationResponse>>(
            $"homes/{homeId}/invitations",
            cancellationToken) ?? new List<TadoInvitationResponse>();

        return response.ToDomainList();
    }

    /// <summary>
    /// Retrieves the incident detection settings for a home. 
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The incident detection configuration.</returns>
    public async Task<IncidentDetection> GetIncidentDetectionAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<TadoIncidentDetectionResponse>(
                $"homes/{homeId}/incidentDetection",
                cancellationToken);

            if (response == null)
                throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                    $"Incident detection for home {homeId} not found.");

            return response.ToDomain();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve incident detection: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the heating circuits configured for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of configured heating circuits.</returns>
    public async Task<IReadOnlyList<HeatingCircuit>> GetHeatingCircuitsAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<List<TadoHeatingCircuitResponse>>(
                $"homes/{homeId}/heatingCircuits",
                cancellationToken) ?? new List<TadoHeatingCircuitResponse>();

            return response.Select(circuit => circuit.ToDomain()).ToList();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve heating circuits: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the heating system configuration for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The configured heating system.</returns>
    public async Task<HeatingSystem> GetHeatingSystemAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<TadoHeatingSystemResponse>(
                $"homes/{homeId}/heatingSystem",
                cancellationToken);

            if (response == null)
                throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                    $"Heating system for home {homeId} not found.");

            return response.ToDomain();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve heating system: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the flow-temperature optimisation settings for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to inspect.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The flow-temperature optimisation settings.</returns>
    public async Task<FlowTemperatureOptimisation> GetFlowTemperatureOptimisationAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        try
        {
            var response = await _httpClient.GetAsync<TadoFlowTemperatureOptimisationResponse>(
                $"homes/{homeId}/flowTemperatureOptimization",
                cancellationToken);

            if (response == null)
                throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                    $"Flow temperature optimisation for home {homeId} not found.");

            return response.ToDomain();
        }
        catch (HttpRequestException ex)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.ServiceUnavailable,
                $"Failed to retrieve flow temperature optimisation: {ex.Message}");
        }
    }

    #endregion

    #region Send Commands

    /// <summary>
    /// Sets the presence state of a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="presence">The presence value to apply, such as HOME or AWAY.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        if (!string.Equals(presence, "HOME", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(presence, "AWAY", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Presence must be HOME or AWAY.", nameof(presence));
        }

        var request = new SetHomePresenceRequest
        {
            HomePresence = presence.ToUpperInvariant()
        };

        await _httpClient.SendAsync(
            $"homes/{homeId}/presenceLock",
            HttpMethod.Put,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent,
            request);
    }

    /// <summary>
    /// Resets the manually set presence state of a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task ResetHomePresenceAsync(int homeId, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        await _httpClient.SendAsync(
            $"homes/{homeId}/presenceLock",
            HttpMethod.Delete,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Sets the geo-tracking away radius for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="awayRadiusInMeters">The distance in meters at which a device is considered away from home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task SetAwayRadiusInMetersAsync(int homeId, double awayRadiusInMeters, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        if (awayRadiusInMeters < 0)
            throw new ArgumentOutOfRangeException(nameof(awayRadiusInMeters), "Away radius must be zero or greater.");

        var request = new SetAwayRadiusInMetersRequest
        {
            AwayRadiusInMeters = awayRadiusInMeters
        };

        await _httpClient.SendAsync(
            $"homes/{homeId}/awayRadiusInMeters",
            HttpMethod.Put,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent,
            request);
    }

    /// <summary>
    /// Enables or disables incident detection for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="enabled">Whether incident detection should be enabled.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task SetIncidentDetectionAsync(int homeId, bool enabled, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        var request = new SetIncidentDetectionRequest
        {
            Enabled = enabled
        };

        await _httpClient.SendAsync(
            $"homes/{homeId}/incidentDetection",
            HttpMethod.Put,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent,
            request);
    }

    /// <summary>
    /// Updates the writable home details for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="homeDetails">The complete writable home details payload.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task SetHomeDetailsAsync(int homeId, House homeDetails, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        if (homeDetails == null)
            throw new ArgumentNullException(nameof(homeDetails));

        Guard.NotNullOrWhiteSpace(homeDetails.Name, nameof(homeDetails));

        if (homeDetails.ContactDetails == null)
            throw new ArgumentException("Home contact details must be provided.", nameof(homeDetails));

        if (homeDetails.Address == null)
            throw new ArgumentException("Home address must be provided.", nameof(homeDetails));

        if (homeDetails.Geolocation == null)
            throw new ArgumentException("Home geolocation must be provided.", nameof(homeDetails));

        var request = SetHomeDetailsRequest.FromDomain(homeDetails);

        await _httpClient.SendAsync(
            $"homes/{homeId}/details",
            HttpMethod.Put,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent,
            request);
    }

    /// <summary>
    /// Updates the maximum flow temperature for a home's boiler optimisation.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="maxFlowTemperature">The maximum flow temperature to apply.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task SetFlowTemperatureOptimisationAsync(int homeId, int maxFlowTemperature, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));

        var request = new SetFlowTemperatureOptimisationRequest
        {
            MaxFlowTemperature = maxFlowTemperature
        };

        await _httpClient.SendAsync(
            $"homes/{homeId}/flowTemperatureOptimization",
            HttpMethod.Put,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent,
            request);
    }

    /// <summary>
    /// Sends an invitation to join a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="email">The email address to invite.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The created invitation.</returns>
    public async Task<Invitation> SendInvitationAsync(int homeId, string email, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));
        Guard.NotNullOrWhiteSpace(email, nameof(email));

        var request = new SendInvitationRequest
        {
            Email = email
        };

        var response = await _httpClient.PostAsync<SendInvitationRequest, TadoInvitationResponse>(
            $"homes/{homeId}/invitations",
            request,
            cancellationToken);

        if (response == null)
        {
            throw new TadoApiException(System.Net.HttpStatusCode.NotFound,
                $"Invitation response for home {homeId} was empty.");
        }

        return response.ToDomain();
    }

    /// <summary>
    /// Revokes a pending invitation.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="invitationToken">The unique invitation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task DeleteInvitationAsync(int homeId, string invitationToken, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));
        Guard.NotNullOrWhiteSpace(invitationToken, nameof(invitationToken));

        await _httpClient.SendAsync(
            $"homes/{homeId}/invitations/{invitationToken}",
            HttpMethod.Delete,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Resends a pending invitation.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="invitationToken">The unique invitation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public async Task ResendInvitationAsync(int homeId, string invitationToken, CancellationToken cancellationToken = default)
    {
        Guard.PositiveId(homeId, nameof(homeId));
        Guard.NotNullOrWhiteSpace(invitationToken, nameof(invitationToken));

        await _httpClient.SendAsync(
            $"homes/{homeId}/invitations/{invitationToken}/resend",
            HttpMethod.Post,
            cancellationToken,
            System.Net.HttpStatusCode.NoContent);
    }

    #endregion

}