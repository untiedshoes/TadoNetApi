using System.Buffers;
using System.Text.Json;
using System.Text;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Converters;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Converters;

/// <summary>
/// Unit tests for JSON enum converters used by infrastructure DTOs.
/// </summary>
public class JsonEnumConvertersTests
{
    /// <summary>
    /// DurationModeConverter maps known strings and returns null for unknown values.
    /// </summary>
    [Fact(DisplayName = "DurationModeConverter maps known and unknown string values")]
    public void DurationModeConverter_MapsKnownAndUnknownStringValues()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new DurationModeConverter());

        var manual = JsonSerializer.Deserialize<DurationModes?>("\"MANUAL\"", options);
        var timer = JsonSerializer.Deserialize<DurationModes?>("\"TIMER\"", options);
        var timedEvent = JsonSerializer.Deserialize<DurationModes?>("\"TADO_MODE\"", options);
        var empty = JsonSerializer.Deserialize<DurationModes?>("\"\"", options);
        var unknown = JsonSerializer.Deserialize<DurationModes?>("\"UNKNOWN\"", options);
        var nonString = JsonSerializer.Deserialize<DurationModes?>("123", options);

        Assert.Equal(DurationModes.UntilNextManualChange, manual);
        Assert.Equal(DurationModes.Timer, timer);
        Assert.Equal(DurationModes.UntilNextTimedEvent, timedEvent);
        Assert.Null(empty);
        Assert.Null(unknown);
        Assert.Null(nonString);
    }

    /// <summary>
    /// DurationModeConverter writes expected tokens including null for unsupported values.
    /// </summary>
    [Fact(DisplayName = "DurationModeConverter writes expected values including null fallback")]
    public void DurationModeConverter_WritesExpectedValues_IncludingNullFallback()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new DurationModeConverter());

        var manual = JsonSerializer.Serialize<DurationModes?>(DurationModes.UntilNextManualChange, options);
        var timer = JsonSerializer.Serialize<DurationModes?>(DurationModes.Timer, options);
        var timedEvent = JsonSerializer.Serialize<DurationModes?>(DurationModes.UntilNextTimedEvent, options);
        var unknown = JsonSerializer.Serialize<DurationModes?>((DurationModes)999, options);
        var nullValue = JsonSerializer.Serialize<DurationModes?>(null, options);

        Assert.Equal("\"MANUAL\"", manual);
        Assert.Equal("\"TIMER\"", timer);
        Assert.Equal("\"TADO_MODE\"", timedEvent);
        Assert.Equal("null", unknown);
        Assert.Equal("null", nullValue);
    }

    /// <summary>
    /// DeviceTypeConverter maps known strings and writes expected values.
    /// </summary>
    [Fact(DisplayName = "DeviceTypeConverter maps and writes expected values")]
    public void DeviceTypeConverter_MapsAndWritesExpectedValues()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new DeviceTypeConverter());

        var heating = JsonSerializer.Deserialize<DeviceTypes?>("\"HEATING\"", options);
        var hotWater = JsonSerializer.Deserialize<DeviceTypes?>("\"HOT_WATER\"", options);
        var empty = JsonSerializer.Deserialize<DeviceTypes?>("\"\"", options);
        var unknown = JsonSerializer.Deserialize<DeviceTypes?>("\"UNKNOWN\"", options);
        var nonString = JsonSerializer.Deserialize<DeviceTypes?>("1", options);

        var serializedHeating = JsonSerializer.Serialize<DeviceTypes?>(DeviceTypes.Heating, options);
        var serializedHotWater = JsonSerializer.Serialize<DeviceTypes?>(DeviceTypes.HotWater, options);
        var serializedUnknown = JsonSerializer.Serialize<DeviceTypes?>((DeviceTypes)999, options);
        var serializedNull = JsonSerializer.Serialize<DeviceTypes?>(null, options);

        Assert.Equal(DeviceTypes.Heating, heating);
        Assert.Equal(DeviceTypes.HotWater, hotWater);
        Assert.Null(empty);
        Assert.Null(unknown);
        Assert.Null(nonString);
        Assert.Equal("\"HEATING\"", serializedHeating);
        Assert.Equal("\"HOT_WATER\"", serializedHotWater);
        Assert.Equal("null", serializedUnknown);
        Assert.Equal("null", serializedNull);
    }

    /// <summary>
    /// PowerStatesConverter maps known strings and writes expected values.
    /// </summary>
    [Fact(DisplayName = "PowerStatesConverter maps and writes expected values")]
    public void PowerStatesConverter_MapsAndWritesExpectedValues()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new PowerStatesConverter());

        var on = JsonSerializer.Deserialize<PowerStates?>("\"ON\"", options);
        var off = JsonSerializer.Deserialize<PowerStates?>("\"OFF\"", options);
        var empty = JsonSerializer.Deserialize<PowerStates?>("\"\"", options);
        var unknown = JsonSerializer.Deserialize<PowerStates?>("\"UNKNOWN\"", options);
        var nonString = JsonSerializer.Deserialize<PowerStates?>("2", options);

        var serializedOn = JsonSerializer.Serialize<PowerStates?>(PowerStates.On, options);
        var serializedOff = JsonSerializer.Serialize<PowerStates?>(PowerStates.Off, options);
        var serializedUnknown = JsonSerializer.Serialize<PowerStates?>((PowerStates)999, options);
        var serializedNull = JsonSerializer.Serialize<PowerStates?>(null, options);

        Assert.Equal(PowerStates.On, on);
        Assert.Equal(PowerStates.Off, off);
        Assert.Null(empty);
        Assert.Null(unknown);
        Assert.Null(nonString);
        Assert.Equal("\"ON\"", serializedOn);
        Assert.Equal("\"OFF\"", serializedOff);
        Assert.Equal("null", serializedUnknown);
        Assert.Equal("null", serializedNull);
    }

    /// <summary>
    /// Converter Write methods emit expected values when invoked directly.
    /// </summary>
    [Fact(DisplayName = "Converter Write methods emit expected values when invoked directly")]
    public void ConverterWriteMethods_EmitExpectedValues_WhenInvokedDirectly()
    {
        var durationJson = WriteDuration(DurationModes.Timer);
        var durationNullJson = WriteDuration(null);
        var durationUnknownJson = WriteDuration((DurationModes)999);

        var deviceTypeJson = WriteDeviceType(DeviceTypes.Heating);
        var deviceTypeNullJson = WriteDeviceType(null);
        var deviceTypeUnknownJson = WriteDeviceType((DeviceTypes)999);

        var powerJson = WritePower(PowerStates.Off);
        var powerNullJson = WritePower(null);
        var powerUnknownJson = WritePower((PowerStates)999);

        Assert.Equal("\"TIMER\"", durationJson);
        Assert.Equal("null", durationNullJson);
        Assert.Equal("null", durationUnknownJson);

        Assert.Equal("\"HEATING\"", deviceTypeJson);
        Assert.Equal("null", deviceTypeNullJson);
        Assert.Equal("null", deviceTypeUnknownJson);

        Assert.Equal("\"OFF\"", powerJson);
        Assert.Equal("null", powerNullJson);
        Assert.Equal("null", powerUnknownJson);
    }

    private static string WriteDuration(DurationModes? value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        new DurationModeConverter().Write(writer, value, new JsonSerializerOptions());
        writer.Flush();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private static string WriteDeviceType(DeviceTypes? value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        new DeviceTypeConverter().Write(writer, value, new JsonSerializerOptions());
        writer.Flush();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }

    private static string WritePower(PowerStates? value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        new PowerStatesConverter().Write(writer, value, new JsonSerializerOptions());
        writer.Flush();
        return Encoding.UTF8.GetString(buffer.WrittenSpan);
    }
}