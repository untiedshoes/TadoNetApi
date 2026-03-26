namespace TadoNetApi.Infrastructure.Config
{
    /// <summary>
    /// Static constants for Tado API endpoints and OAuth2 client settings.
    /// </summary>
    public static class TadoApiEndpoints
    {
        public const string DeviceAuthorizeUrl = "https://login.tado.com/oauth2/device_authorize";
        public const string TokenUrl = "https://auth.tado.com/oauth2/token";
        public const string ApiBaseUrl = "https://my.tado.com/api/v2/";
        public const string ClientId = "tado-web-app";
        public const string ScopeHomeUser = "home.user";
    }
}