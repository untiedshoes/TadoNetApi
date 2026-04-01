namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the current overlay state of a Tado device
    /// </summary>
    public partial class Overlay
    {
        /// <summary>
        /// The current setting applied to the Tado device
        /// </summary>
        public Setting? Setting { get; set; }

        /// <summary>
        /// Information on when the current setting will end
        /// </summary>
        public Termination? Termination { get; set; }
    }
}