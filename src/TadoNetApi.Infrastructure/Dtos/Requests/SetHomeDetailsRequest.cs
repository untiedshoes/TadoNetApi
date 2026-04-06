using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating home details.
/// </summary>
public class SetHomeDetailsRequest
{
    /// <summary>
    /// User-assigned name for the home.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Primary contact details for the home.
    /// </summary>
    [JsonPropertyName("contactDetails")]
    public SetHomeContactDetailsRequest ContactDetails { get; set; } = new();

    /// <summary>
    /// Postal address for the home.
    /// </summary>
    [JsonPropertyName("address")]
    public SetHomeAddressRequest Address { get; set; } = new();

    /// <summary>
    /// Geographic coordinates for the home.
    /// </summary>
    [JsonPropertyName("geolocation")]
    public SetHomeGeolocationRequest Geolocation { get; set; } = new();

    /// <summary>
    /// Maps a domain <see cref="House"/> into the API's writable home-details shape.
    /// </summary>
    public static SetHomeDetailsRequest FromDomain(House house)
        => new()
        {
            Name = house.Name,
            ContactDetails = new SetHomeContactDetailsRequest
            {
                Name = house.ContactDetails?.Name,
                Email = house.ContactDetails?.Email,
                Phone = house.ContactDetails?.Phone
            },
            Address = new SetHomeAddressRequest
            {
                AddressLine1 = house.Address?.AddressLine1,
                AddressLine2 = house.Address?.AddressLine2,
                ZipCode = house.Address?.ZipCode,
                City = house.Address?.City,
                State = house.Address?.State,
                Country = house.Address?.Country
            },
            Geolocation = new SetHomeGeolocationRequest
            {
                Latitude = house.Geolocation?.Latitude,
                Longitude = house.Geolocation?.Longitude
            }
        };
}

/// <summary>
/// Contact details payload for updating home details.
/// </summary>
public class SetHomeContactDetailsRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}

/// <summary>
/// Address payload for updating home details.
/// </summary>
public class SetHomeAddressRequest
{
    [JsonPropertyName("addressLine1")]
    public string? AddressLine1 { get; set; }

    [JsonPropertyName("addressLine2")]
    public object? AddressLine2 { get; set; }

    [JsonPropertyName("zipCode")]
    public string? ZipCode { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public object? State { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

/// <summary>
/// Geolocation payload for updating home details.
/// </summary>
public class SetHomeGeolocationRequest
{
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
}