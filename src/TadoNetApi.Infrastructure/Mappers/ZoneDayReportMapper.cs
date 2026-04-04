using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Provides mapping from zone day-report DTOs to domain entities.
/// </summary>
public static class ZoneDayReportMapper
{
    public static ZoneDayReport ToDomain(this TadoZoneDayReportResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ZoneDayReport
        {
            ZoneType = dto.ZoneType,
            Interval = dto.Interval?.ToDomain(),
            HoursInDay = dto.HoursInDay,
            MeasuredData = dto.MeasuredData?.ToDomain(),
            Stripes = dto.Stripes?.ToDomain(),
            Settings = dto.Settings?.ToDomain(),
            CallForHeat = dto.CallForHeat?.ToDomain(),
            HotWaterProduction = dto.HotWaterProduction?.ToDomain(),
            AcActivity = dto.AcActivity?.ToDomain(),
            Weather = dto.Weather?.ToDomain()
        };
    }

    public static ZoneDayReportInterval ToDomain(this TadoZoneDayReportIntervalResponse dto)
        => new()
        {
            From = dto.From,
            To = dto.To
        };

    public static ZoneDayReportMeasuredData ToDomain(this TadoZoneDayReportMeasuredDataResponse dto)
        => new()
        {
            MeasuringDeviceConnected = dto.MeasuringDeviceConnected?.ToDomain(),
            InsideTemperature = dto.InsideTemperature?.ToDomain(),
            Humidity = dto.Humidity?.ToDomain()
        };

    public static ZoneDayReportBooleanTimeSeries ToDomain(this TadoZoneDayReportBooleanTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            DataIntervals = dto.DataIntervals?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportBooleanDataInterval ToDomain(this TadoZoneDayReportBooleanDataIntervalResponse dto)
        => new()
        {
            From = dto.From,
            To = dto.To,
            Value = dto.Value
        };

    public static ZoneDayReportTemperatureTimeSeries ToDomain(this TadoZoneDayReportTemperatureTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            Min = dto.Min?.ToDomain(),
            Max = dto.Max?.ToDomain(),
            DataPoints = dto.DataPoints?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportTemperatureDataPoint ToDomain(this TadoZoneDayReportTemperatureDataPointResponse dto)
        => new()
        {
            Timestamp = dto.Timestamp,
            Value = dto.Value?.ToDomain()
        };

    public static ZoneDayReportPercentageTimeSeries ToDomain(this TadoZoneDayReportPercentageTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            PercentageUnit = dto.PercentageUnit,
            Min = dto.Min,
            Max = dto.Max,
            DataPoints = dto.DataPoints?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportPercentageDataPoint ToDomain(this TadoZoneDayReportPercentageDataPointResponse dto)
        => new()
        {
            Timestamp = dto.Timestamp,
            Value = dto.Value
        };

    public static ZoneDayReportStripesTimeSeries ToDomain(this TadoZoneDayReportStripesTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            DataIntervals = dto.DataIntervals?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportStripesDataInterval ToDomain(this TadoZoneDayReportStripesDataIntervalResponse dto)
        => new()
        {
            From = dto.From,
            To = dto.To,
            Value = dto.Value?.ToDomain()
        };

    public static ZoneDayReportStripeValue ToDomain(this TadoZoneDayReportStripeValueResponse dto)
        => new()
        {
            StripeType = dto.StripeType,
            Setting = dto.Setting?.ToDomain()
        };

    public static ZoneDayReportSettingTimeSeries ToDomain(this TadoZoneDayReportSettingTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            DataIntervals = dto.DataIntervals?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportSettingDataInterval ToDomain(this TadoZoneDayReportSettingDataIntervalResponse dto)
        => new()
        {
            From = dto.From,
            To = dto.To,
            Value = dto.Value?.ToDomain()
        };

    public static ZoneDayReportSetting ToDomain(this TadoZoneDayReportSettingResponse dto)
        => new()
        {
            Type = dto.Type,
            Power = dto.Power,
            Temperature = dto.Temperature?.ToDomain(),
            Mode = dto.Mode,
            IsBoost = dto.IsBoost
        };

    public static ZoneDayReportCallForHeatTimeSeries ToDomain(this TadoZoneDayReportCallForHeatTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            DataIntervals = dto.DataIntervals?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportCallForHeatDataInterval ToDomain(this TadoZoneDayReportCallForHeatDataIntervalResponse dto)
        => new()
        {
            From = dto.From,
            To = dto.To,
            Value = dto.Value
        };

    public static ZoneDayReportPowerTimeSeries ToDomain(this TadoZoneDayReportPowerTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            DataIntervals = dto.DataIntervals?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportPowerDataInterval ToDomain(this TadoZoneDayReportPowerDataIntervalResponse dto)
        => new()
        {
            From = dto.From,
            To = dto.To,
            Value = dto.Value
        };

    public static ZoneDayReportWeather ToDomain(this TadoZoneDayReportWeatherResponse dto)
        => new()
        {
            Condition = dto.Condition?.ToDomain(),
            Sunny = dto.Sunny?.ToDomain(),
            Slots = dto.Slots?.ToDomain()
        };

    public static ZoneDayReportWeatherConditionTimeSeries ToDomain(this TadoZoneDayReportWeatherConditionTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            DataIntervals = dto.DataIntervals?.Select(item => item.ToDomain()).ToList()
        };

    public static ZoneDayReportWeatherConditionDataInterval ToDomain(this TadoZoneDayReportWeatherConditionDataIntervalResponse dto)
        => new()
        {
            From = dto.From,
            To = dto.To,
            Value = dto.Value?.ToDomain()
        };

    public static ZoneDayReportWeatherConditionValue ToDomain(this TadoZoneDayReportWeatherConditionValueResponse dto)
        => new()
        {
            State = dto.State,
            Temperature = dto.Temperature?.ToDomain()
        };

    public static ZoneDayReportWeatherSlotTimeSeries ToDomain(this TadoZoneDayReportWeatherSlotTimeSeriesResponse dto)
        => new()
        {
            TimeSeriesType = dto.TimeSeriesType,
            ValueType = dto.ValueType,
            Slots = dto.Slots?.ToDictionary(item => item.Key, item => item.Value.ToDomain())
        };

    public static ZoneDayReportWeatherSlot ToDomain(this TadoZoneDayReportWeatherSlotResponse dto)
        => new()
        {
            State = dto.State,
            Temperature = dto.Temperature?.ToDomain()
        };
}