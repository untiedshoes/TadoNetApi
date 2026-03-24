namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Represents a request to update the presence state of a home.
/// </summary>
public class SetHomePresenceRequest
{
    /// <summary>The presence state to set (e.g., HOME, AWAY).</summary>
    public string presence { get; set; } = string.Empty;
}