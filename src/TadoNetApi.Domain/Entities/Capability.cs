namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents a capability of a Tado device, such as temperature control
    /// </summary>
    public class Capability
    {
        /// <summary>
        /// The type of capability (e.g., HEATING, COOLING)
        /// </summary>
        public string? PurpleType { get; set; }

        /// <summary>
        /// The temperature-related capabilities
        /// </summary>
        public Temperatures? Temperatures { get; set; }
    }
}