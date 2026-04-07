namespace TadoNetApi.Domain.Entities {
    /// <summary>
    /// One Tado installation
    /// </summary>
    public class Installation
    {
        /// <summary>
        /// The unique identifier of the installation
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The type of the installation
        /// </summary>
        public string? CurrentType { get; set; }

        /// <summary>
        /// The revision number of the installation
        /// </summary>
        public long? Revision { get; set; }

        /// <summary>
        /// The current state of the installation
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// The list of devices included in the installation
        /// </summary>
        public Device[] Devices { get; set; } = Array.Empty<Device>();
    }
}
    
    
