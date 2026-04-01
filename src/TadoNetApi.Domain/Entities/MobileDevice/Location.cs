namespace TadoNetApi.Domain.Entities.MobileDevice
{
    /// <summary>
    /// Contains the location of a device
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Indicates whether the location data is outdated
        /// </summary>
        public bool? Stale { get; set; }

        /// <summary>
        /// Indicates whether the device is currently at home
        /// </summary>
        public bool? AtHome { get; set; }

        /// <summary>
        /// The direction from the home location to the device
        /// </summary>
        public BearingFromHome? BearingFromHome { get; set; }

        /// <summary>
        /// The relative distance of the device from the home fence
        /// </summary>
        public double? RelativeDistanceFromHomeFence { get; set; }
    }
}