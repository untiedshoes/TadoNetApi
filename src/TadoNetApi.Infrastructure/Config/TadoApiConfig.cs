namespace TadoNetApi.Infrastructure.Config;

/// <summary>
/// Configuration for Tado API transport and retry behavior.
/// </summary>
public class TadoApiConfig
{
    public int MaxRetries { get; set; } = 5;
    public int InitialRetryDelayMs { get; set; } = 1000;
}