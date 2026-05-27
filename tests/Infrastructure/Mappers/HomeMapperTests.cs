using System.Collections.Generic;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="HomeMapper"/>.
/// </summary>
public class HomeMapperTests
{
    /// <summary>
    /// ToDomain maps TadoHomeResponse into Home.
    /// </summary>
    [Fact(DisplayName = "ToDomain maps TadoHomeResponse into Home")]
    public void ToDomain_MapsTadoHomeResponse_IntoHome()
    {
        var dto = new TadoHomeResponse
        {
            Id = 7,
            Name = "Home A"
        };

        var result = HomeMapper.ToDomain(dto);

        Assert.Equal(7, result.Id);
        Assert.Equal("Home A", result.Name);
    }

    /// <summary>
    /// ToDomain maps TadoHouseResponse into Home.
    /// </summary>
    [Fact(DisplayName = "ToDomain maps TadoHouseResponse into Home")]
    public void ToDomain_MapsTadoHouseResponse_IntoHome()
    {
        var dto = new TadoHouseResponse
        {
            Id = 9,
            Name = "Home B"
        };

        var result = HomeMapper.ToDomain(dto);

        Assert.Equal(9, result.Id);
        Assert.Equal("Home B", result.Name);
    }

    /// <summary>
    /// ToDomainList maps all TadoHomeResponse entries into Home entities.
    /// </summary>
    [Fact(DisplayName = "ToDomainList maps all TadoHomeResponse entries")]
    public void ToDomainList_MapsAllTadoHomeResponseEntries()
    {
        var dtos = new List<TadoHomeResponse>
        {
            new() { Id = 1, Name = "One" },
            new() { Id = 2, Name = "Two" }
        };

        var result = HomeMapper.ToDomainList(dtos);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }
}