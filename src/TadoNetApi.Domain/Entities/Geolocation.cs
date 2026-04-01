namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the geographic location of a device or home
    /// </summary>
    public class Geolocation
    {
        /// <summary>
        /// The latitude coordinate
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude coordinate
        /// </summary>
        public double? Longitude { get; set; }
    }
}