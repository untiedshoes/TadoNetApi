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
            SerialNo = dto.SerialNo,
            ShortSerialNo = dto.ShortSerialNo,
            DeviceType = dto.DeviceType,
            CurrentFwVersion = dto.CurrentFwVersion
        };
    }
    
    public static List<Device> ToDomainList(List<TadoDeviceResponse> dtos)
    {
        return dtos.Select(ToDomain).ToList();
    }
}