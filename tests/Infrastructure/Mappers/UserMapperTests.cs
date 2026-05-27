using System.Collections.Generic;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="UserMapper"/>.
/// </summary>
public class UserMapperTests
{
    /// <summary>
    /// ToDomain maps full user payload including nested home and mobile device data.
    /// </summary>
    [Fact(DisplayName = "ToDomain maps full user payload with nested home and mobile device data")]
    public void ToDomain_MapsFullUserPayload_WithNestedHomeAndMobileDeviceData()
    {
        var dto = new TadoUserResponse
        {
            Id = "user-1",
            Name = "Alice",
            Email = "alice@example.com",
            Username = "alice",
            Locale = "en-GB",
            Homes =
            [
                new TadoHomeResponse { Id = 10, Name = "Main Home" }
            ],
            MobileDevices =
            [
                new TadoMobileItemResponse
                {
                    Id = 42,
                    Name = "Alice iPhone",
                    Settings = new TadoMobileSettingsResponse { GeoTrackingEnabled = true },
                    Location = new TadoMobileLocationResponse
                    {
                        Stale = false,
                        AtHome = true,
                        RelativeDistanceFromHomeFence = 0.2,
                        BearingFromHome = new TadoMobileBearingFromHomeResponse { Degrees = 180, Radians = 3.14 }
                    },
                    MobileDeviceDetails = new TadoMobileDetailsResponse
                    {
                        Platform = "iOS",
                        OsVersion = "18.0",
                        Model = "iPhone",
                        Locale = "en-GB"
                    }
                }
            ]
        };

        var result = dto.ToDomain();

        Assert.Equal("user-1", result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal(10, result.Homes?[0].Id);
        Assert.Equal("Main Home", result.Homes?[0].Name);
        Assert.Equal(42, result.MobileDevices?[0].Id);
        Assert.True(result.MobileDevices?[0].Settings?.GeoTrackingEnabled);
        Assert.True(result.MobileDevices?[0].Location?.AtHome);
        Assert.Equal(180, result.MobileDevices?[0].Location?.BearingFromHome?.Degrees);
        Assert.Equal("iOS", result.MobileDevices?[0].MobileDeviceDetails?.Platform);
    }

    /// <summary>
    /// ToDomain supports null nested collections and optional fields.
    /// </summary>
    [Fact(DisplayName = "ToDomain supports null nested collections and optional fields")]
    public void ToDomain_SupportsNullNestedCollections_AndOptionalFields()
    {
        var dto = new TadoUserResponse
        {
            Id = "user-2",
            Name = "Bob",
            Homes = null,
            MobileDevices = null
        };

        var result = dto.ToDomain();

        Assert.Equal("user-2", result.Id);
        Assert.Equal("Bob", result.Name);
        Assert.Null(result.Homes);
        Assert.Null(result.MobileDevices);
    }

    /// <summary>
    /// ToDomainList maps all entries in the source collection.
    /// </summary>
    [Fact(DisplayName = "ToDomainList maps all users in source collection")]
    public void ToDomainList_MapsAllUsers_InSourceCollection()
    {
        var dtos = new List<TadoUserResponse>
        {
            new() { Id = "a", Name = "A" },
            new() { Id = "b", Name = "B" }
        };

        var result = UserMapper.ToDomainList(dtos);

        Assert.Equal(2, result.Count);
        Assert.Equal("a", result[0].Id);
        Assert.Equal("b", result[1].Id);
    }
}