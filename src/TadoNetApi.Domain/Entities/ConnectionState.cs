namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// State of the connection towards a Tado device
    /// </summary>
    public partial class ConnectionState
    {
        public bool Value { get; set; }

        public DateTime Timestamp { get; set; }
    }
}