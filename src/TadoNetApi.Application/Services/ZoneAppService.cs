using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
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

        public Task<ZoneControl> GetZoneControlAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneControlAsync(homeId, zoneId, cancellationToken);

        public Task<DefaultZoneOverlay> GetDefaultZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetDefaultZoneOverlayAsync(homeId, zoneId, cancellationToken);

        public Task<EarlyStart> GetEarlyStartAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetEarlyStartAsync(homeId, zoneId, cancellationToken);

        public Task<AwayConfiguration> GetAwayConfigurationAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetAwayConfigurationAsync(homeId, zoneId, cancellationToken);

        public Task<ZoneDayReport> GetZoneDayReportAsync(int homeId, int zoneId, DateOnly? date = null, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneDayReportAsync(homeId, zoneId, date, cancellationToken);

        public Task<Temperature> GetZoneTemperatureOffsetAsync(Zone zone, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneTemperatureOffsetAsync(zone, cancellationToken);

        public Task<bool> SetEarlyStartAsync(int homeId, int zoneId, bool enabled, CancellationToken cancellationToken = default)
            => _zoneService.SetEarlyStartAsync(homeId, zoneId, enabled, cancellationToken);

        public Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
            => _zoneService.SetHeatingTemperatureCelsiusAsync(homeId, zoneId, temperature, cancellationToken);

        public Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, DurationModes durationMode, TimeSpan? timer = null, CancellationToken cancellationToken = default)
            => _zoneService.SetHeatingTemperatureCelsiusAsync(homeId, zoneId, temperature, durationMode, timer, cancellationToken);
    }
}