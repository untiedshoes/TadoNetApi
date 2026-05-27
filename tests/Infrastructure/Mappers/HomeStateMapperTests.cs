using System;
using System.Collections.Generic;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="HomeStateMapper"/>.
/// </summary>
public class HomeStateMapperTests
{
    /// <summary>
    /// ToDomain maps presence state from DTO.
    /// </summary>
    [Fact(DisplayName = "ToDomain maps presence state from DTO")]
    public void ToDomain_MapsPresenceState_FromDto()
    {
        var dto = new TadoHomeStateResponse { Presence = "HOME" };

        var result = dto.ToDomain();

        Assert.Equal("HOME", result.Presence);
    }

    /// <summary>
    /// ToDomain throws ArgumentNullException when DTO is null.
    /// </summary>
    [Fact(DisplayName = "ToDomain throws ArgumentNullException when DTO is null")]
    public void ToDomain_ThrowsArgumentNullException_WhenDtoIsNull()
    {
        TadoHomeStateResponse dto = null!;

        Assert.Throws<ArgumentNullException>(() => HomeStateMapper.ToDomain(dto));
    }

    /// <summary>
    /// ToDomainList maps all DTO entries.
    /// </summary>
    [Fact(DisplayName = "ToDomainList maps all home state DTO entries")]
    public void ToDomainList_MapsAllHomeStateDtoEntries()
    {
        var dtos = new List<TadoHomeStateResponse>
        {
            new() { Presence = "HOME" },
            new() { Presence = "AWAY" }
        };

        var result = dtos.ToDomainList();

        Assert.Equal(2, result.Count);
        Assert.Equal("HOME", result[0].Presence);
        Assert.Equal("AWAY", result[1].Presence);
    }
}