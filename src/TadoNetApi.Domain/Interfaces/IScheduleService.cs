using TadoNetApi.Domain.Entities;
using System.Threading;

namespace TadoNetApi.Domain.Interfaces;

public interface IScheduleService
{
    /// <summary>
    /// Retrieves the schedule for a specific zone.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="zoneId">The ID of the zone.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of schedules for the specified zone.</returns>
    Task<List<Schedule>> GetZoneScheduleAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a schedule entry for a specific zone.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="zoneId">The ID of the zone.</param>
    /// <param name="entry">The schedule entry to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SetZoneScheduleEntryAsync(int homeId, int zoneId, Schedule entry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the currently active program for a specific zone.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="zoneId">The ID of the zone.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The name of the currently active program for the specified zone.</returns>
    Task<string> GetZoneProgramAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);
}