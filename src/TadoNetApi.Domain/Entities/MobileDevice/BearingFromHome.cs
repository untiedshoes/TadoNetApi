namespace TadoNetApi.Domain.Entities.MobileDevice
{
    /// <summary>
    /// Contains the coordinates relative to the home location where Tado is being used
    /// </summary>
    public partial class BearingFromHome
    {
        /// <summary>
        /// The direction in degrees from the home location
        /// </summary>
        public double? Degrees { get; set; }

        /// <summary>
        /// The direction in radians from the home location
        /// </summary>
        public double? Radians { get; set; }
    }
}