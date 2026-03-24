using System.Runtime.CompilerServices;

namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a device in a Tado zone.
/// </summary>
public class Device
{
    /// <summary>Unique identifier of the device.</summary>
    public int Id { get; set; }

    /// <summary>Name of the device.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Type of device (e.g., THERMOSTAT, SENSOR).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Specific device type string.</summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>Device serial number.</summary>
    public string SerialNo { get; set; } = string.Empty;

    /// <summary>Firmware version of the device.</summary>
    public string FirmwareVersion { get; set; } = string.Empty;

    /// <summary>Identifier of the home associated with the device.</summary>
    public int HomeId { get; set; }

    /// <summary>Identifier of the zone the device belongs to.</summary>
    public int ZoneId { get; set; } 

    /// <summary>Indicates if the device is currently active.</summary>
    public bool Active { get; set; }    

    /// <summary>Email address associated with the device (if applicable).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Indicates if the device has child lock enabled.</summary> 
    public bool ChildLock { get; set; }
}