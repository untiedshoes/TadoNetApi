using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Provides mapping from heating DTOs to domain entities.
/// </summary>
public static class HeatingMapper
{
    public static HeatingCircuit ToDomain(this TadoHeatingCircuitResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new HeatingCircuit
        {
            Number = dto.Number,
            DriverSerialNo = dto.DriverSerialNo,
            DriverShortSerialNo = dto.DriverShortSerialNo
        };
    }

    public static HeatingSystem ToDomain(this TadoHeatingSystemResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new HeatingSystem
        {
            Boiler = dto.Boiler?.ToDomain(),
            UnderfloorHeating = dto.UnderfloorHeating?.ToDomain()
        };
    }

    public static Boiler ToDomain(this TadoBoilerResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Boiler
        {
            Present = dto.Present,
            Id = dto.Id,
            Found = dto.Found
        };
    }

    public static UnderfloorHeating ToDomain(this TadoUnderfloorHeatingResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new UnderfloorHeating
        {
            Present = dto.Present
        };
    }
}