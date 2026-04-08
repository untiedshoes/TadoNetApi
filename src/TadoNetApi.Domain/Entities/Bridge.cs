namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents a Tado Internet Bridge.
    /// </summary>
    public class Bridge
    {
        /// <summary>
        /// Gets or sets the partner metadata when present.
        /// </summary>
        public object? Partner { get; set; }

        /// <summary>
        /// Gets or sets the home ID linked to the bridge.
        /// </summary>
        public int? HomeId { get; set; }
    }
}