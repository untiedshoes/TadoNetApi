
namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Summarized state of a zone
    /// </summary>
    public class ZoneSummary
    {
        /// <summary>
        /// The current state of the Tado device
        /// </summary>
        public Setting? Setting { get; set; }

        /// <summary>
        /// Information on when the current state of the Tado device will end
        /// </summary>
        public Termination? Termination { get; set; }
    }
}