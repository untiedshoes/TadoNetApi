namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Provides friendly names for Tado hardware device type codes.
/// </summary>
public static class DeviceTypeCatalogue
{
    private static readonly IReadOnlyDictionary<string, string> KnownDeviceTypeNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["GW01"] = "Gateway V1",
            ["GW02"] = "Bridge V2",
            ["IB01"] = "Internet Bridge V3+",
            ["BX02"] = "Box V1",
            ["BU01"] = "Extension Kit UK",
            ["EK01"] = "Extension Kit UK",
            ["BR02"] = "Wireless Receiver V3+",
            ["BP02"] = "Wireless Receiver UK V3+",
            ["RU01"] = "Smart Thermostat V3",
            ["RU02"] = "Wired Smart Thermostat V3+",
            ["TS02"] = "Temp Sensor V1",
            ["SU02"] = "Wireless Temperature Sensor V3+",
            ["VA01"] = "Smart Radiator Thermostat V3",
            ["VA02"] = "Smart Radiator Thermostat V3+",
            ["WR01"] = "Smart AC Control V3",
            ["WR02"] = "Smart AC Control V3+"
        };

    /// <summary>
    /// Returns the friendly name for a device type code when known.
    /// </summary>
    public static string? GetFriendlyName(string? deviceTypeCode)
    {
        if (string.IsNullOrWhiteSpace(deviceTypeCode))
            return null;

        var normalisedCode = deviceTypeCode.Trim().ToUpperInvariant();

        if (KnownDeviceTypeNames.TryGetValue(normalisedCode, out var friendlyName))
            return friendlyName;

        // Some devices report a hardware revision suffix, for example SU02B.
        if (normalisedCode.Length > 4 && KnownDeviceTypeNames.TryGetValue(normalisedCode[..4], out friendlyName))
            return friendlyName;

        return null;
    }
}