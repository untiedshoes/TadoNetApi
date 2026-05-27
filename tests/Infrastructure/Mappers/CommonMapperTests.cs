using System;
using System.Collections.Generic;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="CommonMapper"/>.
/// </summary>
public class CommonMapperTests
{
    /// <summary>
    /// CommonMapper maps address contact geolocation and temperature DTOs.
    /// </summary>
    [Fact(DisplayName = "CommonMapper maps address contact geolocation and temperature DTOs")]
    public void CommonMapper_MapsAddressContactGeolocationAndTemperatureDtos()
    {
        var address = new TadoAddressResponse
        {
            AddressLine1 = "1 Main Street",
            AddressLine2 = "Flat 2",
            ZipCode = "AB12",
            City = "London",
            State = "England",
            Country = "GB"
        };
        var contact = new TadoContactDetailsResponse
        {
            Name = "Alex",
            Email = "alex@example.com",
            Phone = "+4412345"
        };
        var geolocation = new TadoGeolocationResponse
        {
            Latitude = 51.5,
            Longitude = -0.12
        };
        var outsideTemperature = new TadoOutsideTemperatureResponse
        {
            Celsius = 12.3,
            Fahrenheit = 54.1,
            Timestamp = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc),
            PurpleType = "OUTSIDE",
            Precision = new TadoPrecisionResponse { Celsius = 0.1, Fahrenheit = 0.2 }
        };
        var temperatures = new TadoTemperaturesResponse
        {
            Celsius = new TadoTemperatureStepsResponse { Min = 5, Max = 25, Step = 0.5 },
            Fahrenheit = new TadoTemperatureStepsResponse { Min = 41, Max = 77, Step = 1 }
        };

        var mappedAddress = address.ToDomain();
        var mappedContact = contact.ToDomain();
        var mappedGeolocation = geolocation.ToDomain();
        var mappedOutsideTemperature = outsideTemperature.ToDomain();
        var mappedTemperatures = temperatures.ToDomain();

        Assert.Equal("1 Main Street", mappedAddress.AddressLine1);
        Assert.Equal("Flat 2", mappedAddress.AddressLine2);
        Assert.Equal("Alex", mappedContact.Name);
        Assert.Equal("alex@example.com", mappedContact.Email);
        Assert.Equal(51.5, mappedGeolocation.Latitude);
        Assert.Equal(-0.12, mappedGeolocation.Longitude);
        Assert.Equal(12.3, mappedOutsideTemperature.Celsius);
        Assert.Equal(0.1, mappedOutsideTemperature.Precision?.Celsius);
        Assert.Equal(25, mappedTemperatures.Celsius?.Max);
        Assert.Equal(77, mappedTemperatures.Fahrenheit?.Max);
    }

    /// <summary>
    /// CommonMapper maps generic lists using the provided mapper delegate.
    /// </summary>
    [Fact(DisplayName = "CommonMapper maps generic lists using the provided mapper")]
    public void CommonMapper_ToDomainList_MapsGenericCollections()
    {
        var result = new[] { "a", "bb", "ccc" }.ToDomainList(static value => value.Length);

        Assert.Equal(new List<int> { 1, 2, 3 }, result);
    }
}