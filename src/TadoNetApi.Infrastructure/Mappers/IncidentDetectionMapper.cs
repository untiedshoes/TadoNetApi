using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Provides mapping from incident detection DTOs to domain entities.
/// </summary>
public static class IncidentDetectionMapper
{
    public static IncidentDetection ToDomain(this TadoIncidentDetectionResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new IncidentDetection
        {
            Enabled = dto.Enabled,
            Supported = dto.Supported
        };
    }
}