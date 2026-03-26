namespace TadoNetApi.Infrastructure.Config;

/// <summary>
/// Configuration for Tado API authentication and retry behavior.
/// </summary>
public class TadoApiConfig
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 5;
    public int InitialRetryDelayMs { get; set; } = 1000;
}