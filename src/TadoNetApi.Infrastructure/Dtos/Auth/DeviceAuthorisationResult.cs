namespace TadoNetApi.Infrastructure.Auth.Dtos
{
    /// <summary>
    /// Represents the result of a device authorization request.
    /// </summary>
    public class DeviceAuthorisationResult
    {
        /// <summary>URL the user must visit.</summary>
        public string VerificationUri { get; set; } = string.Empty;

        /// <summary>User code to enter.</summary>
        public string UserCode { get; set; } = string.Empty;

        /// <summary>Polling interval in seconds.</summary>
        public int Interval { get; set; }

        /// <summary>Device code used internally.</summary>
        public string DeviceCode { get; set; } = string.Empty;
    }
}