

namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// The current state of a Tado device
    /// </summary>
    public partial class Setting
    {
        /// <summary>
        /// Type of Tado device
        /// </summary>
        public Enums.DeviceTypes? DeviceType { get; set; }

        /// <summary>
        /// The power state of the Tado device
        /// </summary>
        public Enums.PowerStates? Power { get; set; }

        /// <summary>
        /// The temperature the Tado device is set to change the zone to
        /// </summary>
        public Temperature? Temperature { get; set; }

        /// <summary>
        /// The operating mode used for air-conditioning capable zones when applicable.
        /// </summary>
        public string? Mode { get; set; }

        /// <summary>
        /// Indicates whether boost mode is enabled when applicable.
        /// </summary>
        public bool? IsBoost { get; set; }
    }
}