namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the address details of a location
    /// </summary>
    public partial class Address
    {
        /// <summary>
        /// The first line of the address
        /// </summary>
        public string? AddressLine1 { get; set; }

        /// <summary>
        /// The second line of the address (optional)
        /// </summary>
        public object? AddressLine2 { get; set; }

        /// <summary>
        /// The postal or ZIP code
        /// </summary>
        public string? ZipCode { get; set; }

        /// <summary>
        /// The city of the address
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// The state or province (optional)
        /// </summary>
        public object? State { get; set; }

        /// <summary>
        /// The country of the address
        /// </summary>
        public string? Country { get; set; }
    }
}