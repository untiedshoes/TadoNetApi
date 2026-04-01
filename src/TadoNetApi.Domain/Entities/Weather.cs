namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// The current weather
    /// </summary>
    public partial class Weather
    {
        /// <summary>
        /// The current solar intensity
        /// </summary>
        public SolarIntensity? SolarIntensity { get; set; }

        /// <summary>
        /// The current outside temperature
        /// </summary>
        public OutsideTemperature? OutsideTemperature { get; set; }

        /// <summary>
        /// The current weather state (e.g., SUNNY, CLOUDY)
        /// </summary>
        public WeatherState? WeatherState { get; set; }
    }
}