using System;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="DeviceMapper"/>.
/// </summary>
public class DeviceMapperTests
{
    /// <summary>
    /// DeviceMapper maps nested device payloads and device-list entries.
    /// </summary>
    [Fact(DisplayName = "DeviceMapper maps nested device payloads and device list entries")]
    public void DeviceMapper_MapsNestedDevicePayloadsAndDeviceListEntries()
    {
        var dto = new TadoDeviceResponse
        {
            DeviceType = "SMART_RADIATOR_THERMOSTAT",
            SerialNo = "SU1234567890",
            ShortSerialNo = "SU123456",
            CurrentFwVersion = "1.2.3",
            ConnectionState = new TadoConnectionStateResponse
            {
                Value = true,
                Timestamp = new DateTime(2026, 4, 5, 10, 0, 0, DateTimeKind.Utc)
            },
            Characteristics = new TadoCharacteristicsResponse
            {
                Capabilities = ["INSIDE_TEMPERATURE_MEASUREMENT"]
            },
            Duties = ["ZONE_UI", "ZONE_LEADER"],
            MountingState = new TadoMountingStateResponse
            {
                Value = "MOUNTED",
                Timestamp = new DateTime(2026, 4, 5, 9, 0, 0, DateTimeKind.Utc)
            },
            BatteryState = "NORMAL",
            ChildLockEnabled = true
        };

        var mappedDevice = dto.ToDomain();
        var mappedDevices = new[] { dto }.ToDomainList();

        var entry = new TadoDeviceListItemResponse
        {
            Type = "GW03",
            Device = dto,
            Zone = new TadoDeviceListZoneResponse
            {
                Discriminator = 7,
                Duties = ["ZONE_UI"]
            }
        };

        var mappedEntry = entry.ToDomain();
        var mappedEntries = new[] { entry }.ToDomainList();

        Assert.Equal("SMART_RADIATOR_THERMOSTAT", mappedDevice.DeviceType);
        Assert.Equal("SU1234567890", mappedDevice.SerialNo);
        Assert.True(mappedDevice.ConnectionState?.Value);
        Assert.Equal("INSIDE_TEMPERATURE_MEASUREMENT", Assert.Single(mappedDevice.Characteristics!.Capabilities!));
        Assert.Equal("MOUNTED", mappedDevice.MountingState?.Value);
        Assert.True(mappedDevice.ChildLockEnabled);
        Assert.Single(mappedDevices);
        Assert.Equal("GW03", mappedEntry.Type);
        Assert.Equal(7, mappedEntry.ZoneId);
        Assert.Equal("ZONE_UI", Assert.Single(mappedEntry.ZoneDuties!));
        Assert.Single(mappedEntries);
    }
}