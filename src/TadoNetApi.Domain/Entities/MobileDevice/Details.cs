namespace TadoNetApi.Domain.Entities.MobileDevice
{
    /// <summary>
    /// Contains detailed information about a device connected to Tado
    /// </summary>
    public class Details
    {
        /// <summary>
        /// The platform of the mobile device (e.g., iOS, Android)
        /// </summary>
        public string? Platform { get; set; }

        /// <summary>
        /// The operating system version of the mobile device
        /// </summary>
        public string? OsVersion { get; set; }

        /// <summary>
        /// The model of the mobile device
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// The locale setting of the mobile device
        /// </summary>
        public string? Locale { get; set; }
    }
}