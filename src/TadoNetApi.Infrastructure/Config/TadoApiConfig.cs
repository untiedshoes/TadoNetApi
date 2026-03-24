namespace TadoNetApi.Infrastructure.Config;

/// <summary>
/// Configuration for Tado API authentication and retry behavior.
/// </summary>
public class TadoApiConfig
{
    /// <summary>Tado account username (email).</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Tado account password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Max retries when rate-limited (HTTP 429).</summary>
    public int MaxRetries { get; set; } = 5;

    /// <summary>Initial delay for exponential backoff (milliseconds).</summary>
    public int InitialRetryDelayMs { get; set; } = 1000;
}