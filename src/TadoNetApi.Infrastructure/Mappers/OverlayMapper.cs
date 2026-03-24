using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Maps Tado API overlay DTOs to domain entities.
/// </summary>
public static class OverlayMapper
{
    /// <summary>
    /// Maps TadoOverlayResponse to Overlay domain entity.
    /// </summary>
    public static Overlay ToDomain(TadoOverlayResponse dto)
    {
        return new Overlay
        {
            Type = dto.Type ?? string.Empty,
            TargetTemperature = dto.Temperature ?? 0,
            EndTime = dto.EndTime
        };
    }
}