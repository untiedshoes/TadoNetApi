using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;

namespace TadoNetApi.Infrastructure.Services;

/// <summary>
/// Concrete implementation of IDeviceService using the Tado API.
/// </summary>
public class TadoDeviceService : IDeviceService
{
    private readonly ITadoHttpClient _httpClient;

    public TadoDeviceService(ITadoHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<List<Device>> GetDevicesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
    {
        var dtos = await _httpClient.GetAsync<List<TadoDeviceResponse>>($"homes/{homeId}/zones/{zoneId}/devices", cancellationToken);
        return dtos == null ? new List<Device>() : DeviceMapper.ToDomainList(dtos);
    }

    /// <inheritdoc/>
    public async Task<Device?> GetDeviceAsync(int homeId, int zoneId, int deviceId, CancellationToken cancellationToken = default)
    {
        var dto = await _httpClient.GetAsync<TadoDeviceResponse>($"homes/{homeId}/zones/{zoneId}/devices/{deviceId}", cancellationToken);
        return dto == null ? null : DeviceMapper.ToDomain(dto);
    }

    /// <inheritdoc/>
    public async Task SetDeviceChildLockAsync(int homeId, int zoneId, int deviceId, bool lockEnabled)
    {
        var request = new SetChildLockRequest
        {
            ChildLock = lockEnabled
        };

        await _httpClient.PostAsync<SetChildLockRequest, object>(
            $"homes/{homeId}/zones/{zoneId}/devices/{deviceId}/childLock",
            request
        );
    }

}