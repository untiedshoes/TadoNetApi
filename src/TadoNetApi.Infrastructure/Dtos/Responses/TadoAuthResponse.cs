namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the response from the Tado API when authenticating a user.
/// </summary>
public class TadoAuthResponse
{
    /// <summary>The access token to be used for subsequent API requests.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>The refresh token to obtain a new access token after expiration.</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Number of seconds before the access token expires.</summary>
    public int ExpiresIn { get; set; }

    /// <summary>The type of token, typically 'Bearer'.</summary>
    public string TokenType { get; set; } = "Bearer";
}