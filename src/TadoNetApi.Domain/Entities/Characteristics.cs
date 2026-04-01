namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Characteristics of a device
    /// </summary>
    public class Characteristics
    {
        /// <summary>
        /// The list of capabilities supported by the device
        /// </summary>
        public string[]? Capabilities { get; set; }
    }
}