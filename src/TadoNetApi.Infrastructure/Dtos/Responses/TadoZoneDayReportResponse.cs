using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the API response for a zone day report.
/// </summary>
public class TadoZoneDayReportResponse
{
    [JsonPropertyName("zoneType")]
    public string? ZoneType { get; set; }

    [JsonPropertyName("interval")]
    public TadoZoneDayReportIntervalResponse? Interval { get; set; }

    [JsonPropertyName("hoursInDay")]
    public int? HoursInDay { get; set; }

    [JsonPropertyName("measuredData")]
    public TadoZoneDayReportMeasuredDataResponse? MeasuredData { get; set; }

    [JsonPropertyName("stripes")]
    public TadoZoneDayReportStripesTimeSeriesResponse? Stripes { get; set; }

    [JsonPropertyName("settings")]
    public TadoZoneDayReportSettingTimeSeriesResponse? Settings { get; set; }

    [JsonPropertyName("callForHeat")]
    public TadoZoneDayReportCallForHeatTimeSeriesResponse? CallForHeat { get; set; }

    [JsonPropertyName("hotWaterProduction")]
    public TadoZoneDayReportBooleanTimeSeriesResponse? HotWaterProduction { get; set; }

    [JsonPropertyName("acActivity")]
    public TadoZoneDayReportPowerTimeSeriesResponse? AcActivity { get; set; }

    [JsonPropertyName("weather")]
    public TadoZoneDayReportWeatherResponse? Weather { get; set; }
}

public class TadoZoneDayReportIntervalResponse
{
    [JsonPropertyName("from")]
    public DateTime? From { get; set; }

    [JsonPropertyName("to")]
    public DateTime? To { get; set; }
}

public class TadoZoneDayReportMeasuredDataResponse
{
    [JsonPropertyName("measuringDeviceConnected")]
    public TadoZoneDayReportBooleanTimeSeriesResponse? MeasuringDeviceConnected { get; set; }

    [JsonPropertyName("insideTemperature")]
    public TadoZoneDayReportTemperatureTimeSeriesResponse? InsideTemperature { get; set; }

    [JsonPropertyName("humidity")]
    public TadoZoneDayReportPercentageTimeSeriesResponse? Humidity { get; set; }
}

public class TadoZoneDayReportBooleanTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("dataIntervals")]
    public List<TadoZoneDayReportBooleanDataIntervalResponse>? DataIntervals { get; set; }
}

public class TadoZoneDayReportBooleanDataIntervalResponse : TadoZoneDayReportIntervalResponse
{
    [JsonPropertyName("value")]
    public bool? Value { get; set; }
}

public class TadoZoneDayReportTemperatureTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("min")]
    public TadoTemperatureResponse? Min { get; set; }

    [JsonPropertyName("max")]
    public TadoTemperatureResponse? Max { get; set; }

    [JsonPropertyName("dataPoints")]
    public List<TadoZoneDayReportTemperatureDataPointResponse>? DataPoints { get; set; }
}

public class TadoZoneDayReportTemperatureDataPointResponse
{
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("value")]
    public TadoTemperatureResponse? Value { get; set; }
}

public class TadoZoneDayReportPercentageTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("percentageUnit")]
    public string? PercentageUnit { get; set; }

    [JsonPropertyName("min")]
    public double? Min { get; set; }

    [JsonPropertyName("max")]
    public double? Max { get; set; }

    [JsonPropertyName("dataPoints")]
    public List<TadoZoneDayReportPercentageDataPointResponse>? DataPoints { get; set; }
}

public class TadoZoneDayReportPercentageDataPointResponse
{
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    [JsonPropertyName("value")]
    public double? Value { get; set; }
}

public class TadoZoneDayReportStripesTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("dataIntervals")]
    public List<TadoZoneDayReportStripesDataIntervalResponse>? DataIntervals { get; set; }
}

public class TadoZoneDayReportStripesDataIntervalResponse : TadoZoneDayReportIntervalResponse
{
    [JsonPropertyName("value")]
    public TadoZoneDayReportStripeValueResponse? Value { get; set; }
}

public class TadoZoneDayReportStripeValueResponse
{
    [JsonPropertyName("stripeType")]
    public string? StripeType { get; set; }

    [JsonPropertyName("setting")]
    public TadoZoneDayReportSettingResponse? Setting { get; set; }
}

public class TadoZoneDayReportSettingTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("dataIntervals")]
    public List<TadoZoneDayReportSettingDataIntervalResponse>? DataIntervals { get; set; }
}

public class TadoZoneDayReportSettingDataIntervalResponse : TadoZoneDayReportIntervalResponse
{
    [JsonPropertyName("value")]
    public TadoZoneDayReportSettingResponse? Value { get; set; }
}

public class TadoZoneDayReportSettingResponse
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("power")]
    public string? Power { get; set; }

    [JsonPropertyName("temperature")]
    public TadoTemperatureResponse? Temperature { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("isBoost")]
    public bool? IsBoost { get; set; }
}

public class TadoZoneDayReportCallForHeatTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("dataIntervals")]
    public List<TadoZoneDayReportCallForHeatDataIntervalResponse>? DataIntervals { get; set; }
}

public class TadoZoneDayReportCallForHeatDataIntervalResponse : TadoZoneDayReportIntervalResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public class TadoZoneDayReportPowerTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("dataIntervals")]
    public List<TadoZoneDayReportPowerDataIntervalResponse>? DataIntervals { get; set; }
}

public class TadoZoneDayReportPowerDataIntervalResponse : TadoZoneDayReportIntervalResponse
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public class TadoZoneDayReportWeatherResponse
{
    [JsonPropertyName("condition")]
    public TadoZoneDayReportWeatherConditionTimeSeriesResponse? Condition { get; set; }

    [JsonPropertyName("sunny")]
    public TadoZoneDayReportBooleanTimeSeriesResponse? Sunny { get; set; }

    [JsonPropertyName("slots")]
    public TadoZoneDayReportWeatherSlotTimeSeriesResponse? Slots { get; set; }
}

public class TadoZoneDayReportWeatherConditionTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("dataIntervals")]
    public List<TadoZoneDayReportWeatherConditionDataIntervalResponse>? DataIntervals { get; set; }
}

public class TadoZoneDayReportWeatherConditionDataIntervalResponse : TadoZoneDayReportIntervalResponse
{
    [JsonPropertyName("value")]
    public TadoZoneDayReportWeatherConditionValueResponse? Value { get; set; }
}

public class TadoZoneDayReportWeatherConditionValueResponse
{
    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("temperature")]
    public TadoTemperatureResponse? Temperature { get; set; }
}

public class TadoZoneDayReportWeatherSlotTimeSeriesResponse
{
    [JsonPropertyName("timeSeriesType")]
    public string? TimeSeriesType { get; set; }

    [JsonPropertyName("valueType")]
    public string? ValueType { get; set; }

    [JsonPropertyName("slots")]
    public Dictionary<string, TadoZoneDayReportWeatherSlotResponse>? Slots { get; set; }
}

public class TadoZoneDayReportWeatherSlotResponse
{
    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("temperature")]
    public TadoTemperatureResponse? Temperature { get; set; }
}