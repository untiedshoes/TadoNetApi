namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Termination information for current zone setting.
    /// </summary>
    public class Termination
    {
        public string? Type { get; set; }
        public DateTime? ProjectedExpiry { get; set; }
        public DateTime? Expiry { get; set; }
        public int? DurationInSeconds { get; set; }
    }
}