namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents boiler presence and identity returned by bridge-scoped boiler info endpoints.
    /// </summary>
    public class BoilerInfo
    {
        /// <summary>
        /// Gets or sets whether a boiler is present.
        /// </summary>
        public bool? BoilerPresent { get; set; }

        /// <summary>
        /// Gets or sets the Tado boiler identifier when known.
        /// </summary>
        public int? BoilerId { get; set; }
    }
}