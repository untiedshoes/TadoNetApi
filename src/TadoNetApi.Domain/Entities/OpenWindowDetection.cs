namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Open Window Detection settings
    /// </summary>
    public partial class OpenWindowDetection
    {
        /// <summary>
        /// Indicates whether the open window detection feature is supported
        /// </summary>
        public bool? Supported { get; set; }

        /// <summary>
        /// Indicates whether the open window detection feature is enabled
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// The timeout duration in seconds for open window detection
        /// </summary>
        public long? TimeoutInSeconds { get; set; }
    }
}