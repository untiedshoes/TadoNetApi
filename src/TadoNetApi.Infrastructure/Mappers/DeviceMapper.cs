using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Maps Tado API Device DTOs to Domain entities.
/// </summary>
public static class DeviceMapper
{
    /// <summary>
     /// Maps a TadoDeviceResponse to a Domain Device entity.
     /// </summary>
    public static Device ToDomain(TadoDeviceResponse dto)
    {
        return new Device
        {
            Id = dto.Id,
            Name = dto.Name,
            SerialNo = dto.SerialNo ?? string.Empty,
            DeviceType = dto.DeviceType ?? string.Empty,
            ChildLock = dto.ChildLock ?? false
        };
    }
    
    public static List<Device> ToDomainList(List<TadoDeviceResponse> dtos)
    {
        return dtos.Select(ToDomain).ToList();
    }
}