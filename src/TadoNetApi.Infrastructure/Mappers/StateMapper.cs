using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping between Tado API DTOs and domain State entities.
    /// </summary>
    public static class StateMapper
    {
        /// <summary>
        /// Maps a <see cref="TadoStateResponse"/> to a <see cref="State"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A fully mapped domain <see cref="State"/>.</returns>
        public static State ToDomain(this TadoStateResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new State
            {
                TadoMode = dto.TadoMode,
                GeolocationOverride = dto.GeolocationOverride,
                GeolocationOverrideDisableTime = dto.GeolocationOverrideDisableTime,
                Preparation = dto.Preparation,
                Setting = dto.Setting?.ToDomain(),
                OverlayType = dto.OverlayType,
                Overlay = dto.Overlay?.ToDomain(),
                OpenWindow = dto.OpenWindow,
                OpenWindowDetected = dto.OpenWindowDetected,
                Link = dto.Link?.ToDomain(),
                ActivityDataPoints = dto.ActivityDataPoints?.ToDomain(),
                SensorDataPoints = dto.SensorDataPoints?.ToDomain()
            };
        }

        /// <summary>
        /// Maps a <see cref="TadoSettingResponse"/> to a <see cref="Setting"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Setting"/>.</returns>
        public static Setting ToDomain(this TadoSettingResponse dto)
            => new() { DeviceType = dto.DeviceType, Power = dto.Power, Temperature = dto.Temperature?.ToDomain() };

        /// <summary>
        /// Maps a <see cref="TadoTemperatureResponse"/> to a <see cref="Temperature"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Temperature"/>.</returns>
        public static Temperature ToDomain(this TadoTemperatureResponse dto)
            => new() { Celsius = dto.Celsius, Fahrenheit = dto.Fahrenheit };

        /// <summary>
        /// Maps a <see cref="TadoOverlayResponse"/> to an <see cref="Overlay"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Overlay"/>.</returns>
        public static Overlay ToDomain(this TadoOverlayResponse dto)
            => new()
            {
                Setting = dto.Setting?.ToDomain(),
                Termination = dto.Termination?.ToDomain()
            };

        /// <summary>
        /// Maps a <see cref="TadoLinkResponse"/> to a <see cref="Link"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Link"/>.</returns>
        public static Link ToDomain(this TadoLinkResponse dto)
            => new() { State = dto.State };

        /// <summary>
        /// Maps a <see cref="TadoActivityDataPointsResponse"/> to <see cref="ActivityDataPoints"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="ActivityDataPoints"/>.</returns>
        public static ActivityDataPoints ToDomain(this TadoActivityDataPointsResponse dto)
            => new() { HeatingPower = dto.HeatingPower?.ToDomain() };

        /// <summary>
        /// Maps a <see cref="TadoSensorDataPointsResponse"/> to <see cref="SensorDataPoints"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="SensorDataPoints"/>.</returns>
        public static SensorDataPoints ToDomain(this TadoSensorDataPointsResponse dto)
            => new()
            {
                InsideTemperature = dto.InsideTemperature?.ToDomain(),
                Humidity = dto.Humidity?.ToDomain()
            };

        /// <summary>
        /// Maps a <see cref="TadoTerminationResponse"/> to a <see cref="Termination"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Termination"/>.</returns>
        public static Termination ToDomain(this TadoTerminationResponse dto)
            => new()
            {
                Type = dto.CurrentType?.ToString(),
                ProjectedExpiry = dto.ProjectedExpiry,
                Expiry = dto.Expiry,
                DurationInSeconds = dto.DurationInSeconds
            };

        /// <summary>
        /// Maps a <see cref="TadoHeatingPowerResponse"/> to a <see cref="HeatingPower"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="HeatingPower"/>.</returns>
        public static HeatingPower ToDomain(this TadoHeatingPowerResponse dto)
            => new() { CurrentType = dto.CurrentType, Percentage = dto.Percentage, Timestamp = dto.Timestamp };

        /// <summary>
        /// Maps a <see cref="TadoInsideTemperatureResponse"/> to an <see cref="InsideTemperature"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="InsideTemperature"/>.</returns>
        public static InsideTemperature ToDomain(this TadoInsideTemperatureResponse dto)
            => new() { Celsius = dto.Celsius, Fahrenheit = dto.Fahrenheit, Timestamp = dto.Timestamp, Precision = dto.Precision?.ToDomain() };

        /// <summary>
        /// Maps a <see cref="TadoHumidityResponse"/> to a <see cref="Humidity"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Humidity"/>.</returns>
        public static Humidity ToDomain(this TadoHumidityResponse dto)
            => new() { Percentage = dto.Percentage, Timestamp = dto.Timestamp };

        /// <summary>
        /// Maps a <see cref="TadoSolarIntensityResponse"/> to a <see cref="SolarIntensity"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="SolarIntensity"/>.</returns>
        public static SolarIntensity ToDomain(this TadoSolarIntensityResponse dto)
            => new() { Percentage = dto.Percentage, Timestamp = dto.Timestamp };

        /// <summary>
        /// Maps a <see cref="TadoPrecisionResponse"/> to a <see cref="Precision"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Precision"/>.</returns>
        public static Precision ToDomain(this TadoPrecisionResponse dto)
            => new() { Celsius = dto.Celsius, Fahrenheit = dto.Fahrenheit };

        /// <summary>
        /// Maps a list of <see cref="TadoStateResponse"/> to a list of <see cref="State"/> domain entities.
        /// </summary>
        /// <param name="dtos">The list of API DTOs.</param>
        /// <returns>A list of domain <see cref="State"/> entities.</returns>
        public static List<State> ToDomainList(this IEnumerable<TadoStateResponse> dtos)
        {
            return dtos.Select(d => d.ToDomain()).ToList();
        }
    }
}