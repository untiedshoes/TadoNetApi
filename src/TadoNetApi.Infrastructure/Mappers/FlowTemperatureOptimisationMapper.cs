using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Provides mapping from flow-temperature optimisation DTOs to domain entities.
/// </summary>
public static class FlowTemperatureOptimisationMapper
{
    public static FlowTemperatureOptimisation ToDomain(this TadoFlowTemperatureOptimisationResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new FlowTemperatureOptimisation
        {
            HasMultipleBoilerControlDevices = dto.HasMultipleBoilerControlDevices,
            MaxFlowTemperature = dto.MaxFlowTemperature,
            MaxFlowTemperatureConstraints = dto.MaxFlowTemperatureConstraints?.ToDomain(),
            AutoAdaptation = dto.AutoAdaptation?.ToDomain(),
            OpenThermDeviceSerialNumber = dto.OpenThermDeviceSerialNumber
        };
    }

    public static FlowTemperatureOptimisationConstraints ToDomain(this TadoFlowTemperatureOptimisationConstraintsResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new FlowTemperatureOptimisationConstraints
        {
            Min = dto.Min,
            Max = dto.Max
        };
    }

    public static FlowTemperatureOptimisationAutoAdaptation ToDomain(this TadoFlowTemperatureOptimisationAutoAdaptationResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new FlowTemperatureOptimisationAutoAdaptation
        {
            Enabled = dto.Enabled,
            MaxFlowTemperature = dto.MaxFlowTemperature
        };
    }
}