using System;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="CapabilityMapper"/>.
/// </summary>
public class CapabilityMapperTests
{
    /// <summary>
    /// CapabilityMapper maps nested temperature ranges and list payloads.
    /// </summary>
    [Fact(DisplayName = "CapabilityMapper maps nested temperature ranges and list payloads")]
    public void CapabilityMapper_MapsNestedTemperatureRangesAndListPayloads()
    {
        var dto = new TadoCapabilityResponse
        {
            PurpleType = "HEATING",
            Temperatures = new TadoTemperaturesResponse
            {
                Celsius = new TadoTemperatureStepsResponse { Min = 5, Max = 25, Step = 1 },
                Fahrenheit = new TadoTemperatureStepsResponse { Min = 41, Max = 77, Step = 1 }
            }
        };

        var mapped = dto.ToDomain();
        var mappedList = new[] { dto }.ToDomainList();

        Assert.Equal("HEATING", mapped.PurpleType);
        Assert.Equal(25, mapped.Temperatures?.Celsius?.Max);
        Assert.Equal(77, mapped.Temperatures?.Fahrenheit?.Max);
        Assert.Single(mappedList);
    }

    /// <summary>
    /// CapabilityMapper throws ArgumentNullException for null DTOs.
    /// </summary>
    [Fact(DisplayName = "CapabilityMapper throws ArgumentNullException for null DTOs")]
    public void CapabilityMapper_ThrowsArgumentNullException_ForNullDtos()
    {
        Assert.Throws<ArgumentNullException>(() => CapabilityMapper.ToDomain(null!));
    }
}