using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services
{
    /// <summary>
    /// Provides application-level orchestration for zone operations.
    /// </summary>
    public class ZoneAppService
    {
        private readonly IZoneService _zoneService;

        public ZoneAppService(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        public Task<IReadOnlyList<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default)
            => _zoneService.GetZonesAsync(homeId, cancellationToken);

        public Task<Zone> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneAsync(homeId, zoneId, cancellationToken);

        public Task<State> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneStateAsync(homeId, zoneId, cancellationToken);

        public Task<ZoneSummary?> GetZoneSummaryAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneSummaryAsync(homeId, zoneId, cancellationToken);

        public Task<IReadOnlyList<Capability>> GetZoneCapabilitiesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneCapabilitiesAsync(homeId, zoneId, cancellationToken);

        public Task<EarlyStart> GetEarlyStartAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetEarlyStartAsync(homeId, zoneId, cancellationToken);

        public Task<Temperature> GetZoneTemperatureOffsetAsync(Zone zone, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneTemperatureOffsetAsync(zone, cancellationToken);
    }
}