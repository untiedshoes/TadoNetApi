namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Details about the current configuration of the dazzle mode (animation when changing the temperature)
    /// </summary>
    public partial class DazzleMode
    {
        /// <summary>
        /// Indicates whether the dazzle mode feature is supported
        /// </summary>
        public bool? Supported { get; set; }

        /// <summary>
        /// Indicates whether the dazzle mode feature is currently enabled
        /// </summary>
        public bool? Enabled { get; set; }
    }
}