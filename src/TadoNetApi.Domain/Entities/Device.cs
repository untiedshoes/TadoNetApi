using System.Runtime.CompilerServices;

namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a device in a Tado zone.
/// </summary>
public class Device
{
    /// <summary>Device serial number.</summary>
    public string SerialNo { get; set; } = string.Empty;

    /// <summary>Short serial number (last 4 characters of the serial number).</summary>
    public string ShortSerialNo { get; set; } = string.Empty;

    /// <summary>Type of device (e.g., THERMOSTAT, SENSOR).</summary>
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>Firmware version of the device.</summary>
    public string CurrentFwVersion { get; set; } = string.Empty;
}