namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the day report returned for a zone.
/// </summary>
public class ZoneDayReport
{
    public string? ZoneType { get; set; }

    public ZoneDayReportInterval? Interval { get; set; }

    public int? HoursInDay { get; set; }

    public ZoneDayReportMeasuredData? MeasuredData { get; set; }

    public ZoneDayReportStripesTimeSeries? Stripes { get; set; }

    public ZoneDayReportSettingTimeSeries? Settings { get; set; }

    public ZoneDayReportCallForHeatTimeSeries? CallForHeat { get; set; }

    public ZoneDayReportBooleanTimeSeries? HotWaterProduction { get; set; }

    public ZoneDayReportPowerTimeSeries? AcActivity { get; set; }

    public ZoneDayReportWeather? Weather { get; set; }
}

public class ZoneDayReportInterval
{
    public DateTime? From { get; set; }

    public DateTime? To { get; set; }
}

public class ZoneDayReportMeasuredData
{
    public ZoneDayReportBooleanTimeSeries? MeasuringDeviceConnected { get; set; }

    public ZoneDayReportTemperatureTimeSeries? InsideTemperature { get; set; }

    public ZoneDayReportPercentageTimeSeries? Humidity { get; set; }
}

public class ZoneDayReportBooleanTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public IReadOnlyList<ZoneDayReportBooleanDataInterval>? DataIntervals { get; set; }
}

public class ZoneDayReportBooleanDataInterval : ZoneDayReportInterval
{
    public bool? Value { get; set; }
}

public class ZoneDayReportTemperatureTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public Temperature? Min { get; set; }

    public Temperature? Max { get; set; }

    public IReadOnlyList<ZoneDayReportTemperatureDataPoint>? DataPoints { get; set; }
}

public class ZoneDayReportTemperatureDataPoint
{
    public DateTime? Timestamp { get; set; }

    public Temperature? Value { get; set; }
}

public class ZoneDayReportPercentageTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public string? PercentageUnit { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public IReadOnlyList<ZoneDayReportPercentageDataPoint>? DataPoints { get; set; }
}

public class ZoneDayReportPercentageDataPoint
{
    public DateTime? Timestamp { get; set; }

    public double? Value { get; set; }
}

public class ZoneDayReportStripesTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public IReadOnlyList<ZoneDayReportStripesDataInterval>? DataIntervals { get; set; }
}

public class ZoneDayReportStripesDataInterval : ZoneDayReportInterval
{
    public ZoneDayReportStripeValue? Value { get; set; }
}

public class ZoneDayReportStripeValue
{
    public string? StripeType { get; set; }

    public ZoneDayReportSetting? Setting { get; set; }
}

public class ZoneDayReportSettingTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public IReadOnlyList<ZoneDayReportSettingDataInterval>? DataIntervals { get; set; }
}

public class ZoneDayReportSettingDataInterval : ZoneDayReportInterval
{
    public ZoneDayReportSetting? Value { get; set; }
}

public class ZoneDayReportSetting
{
    public string? Type { get; set; }

    public string? Power { get; set; }

    public Temperature? Temperature { get; set; }

    public string? Mode { get; set; }

    public bool? IsBoost { get; set; }
}

public class ZoneDayReportCallForHeatTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public IReadOnlyList<ZoneDayReportCallForHeatDataInterval>? DataIntervals { get; set; }
}

public class ZoneDayReportCallForHeatDataInterval : ZoneDayReportInterval
{
    public string? Value { get; set; }
}

public class ZoneDayReportPowerTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public IReadOnlyList<ZoneDayReportPowerDataInterval>? DataIntervals { get; set; }
}

public class ZoneDayReportPowerDataInterval : ZoneDayReportInterval
{
    public string? Value { get; set; }
}

public class ZoneDayReportWeather
{
    public ZoneDayReportWeatherConditionTimeSeries? Condition { get; set; }

    public ZoneDayReportBooleanTimeSeries? Sunny { get; set; }

    public ZoneDayReportWeatherSlotTimeSeries? Slots { get; set; }
}

public class ZoneDayReportWeatherConditionTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public IReadOnlyList<ZoneDayReportWeatherConditionDataInterval>? DataIntervals { get; set; }
}

public class ZoneDayReportWeatherConditionDataInterval : ZoneDayReportInterval
{
    public ZoneDayReportWeatherConditionValue? Value { get; set; }
}

public class ZoneDayReportWeatherConditionValue
{
    public string? State { get; set; }

    public Temperature? Temperature { get; set; }
}

public class ZoneDayReportWeatherSlotTimeSeries
{
    public string? TimeSeriesType { get; set; }

    public string? ValueType { get; set; }

    public IReadOnlyDictionary<string, ZoneDayReportWeatherSlot>? Slots { get; set; }
}

public class ZoneDayReportWeatherSlot
{
    public string? State { get; set; }

    public Temperature? Temperature { get; set; }
}