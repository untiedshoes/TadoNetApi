namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// State of the mounted Tado device
    /// </summary>
    public class MountingState
    {
        /// <summary>
        /// The current mounting state of the device (e.g., MOUNTED, UNMOUNTED)
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// The timestamp when the mounting state was recorded
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}