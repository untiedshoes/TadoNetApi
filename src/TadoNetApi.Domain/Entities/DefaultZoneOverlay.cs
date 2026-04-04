namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the default overlay configuration for a zone.
    /// </summary>
    public class DefaultZoneOverlay
    {
        /// <summary>
        /// The default termination condition applied when an overlay is created from a device interaction.
        /// </summary>
        public Termination? TerminationCondition { get; set; }
    }
}