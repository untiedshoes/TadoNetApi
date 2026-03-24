namespace TadoNetApi.Infrastructure.Config
{
    /// <summary>
    /// Contains all Tado API endpoint URLs and static constants.
    /// </summary>
    public static class TadoApiEndpoints
    {
        // OAuth2 Device Authorization
        public const string DeviceAuthorizeUrl = "https://login.tado.com/oauth2/device_authorize";
        public const string TokenUrl = "https://auth.tado.com/oauth2/token";

        // API base URL
        public const string ApiBaseUrl = "https://my.tado.com/api/v2/";

        // Client ID used in Tado web app
        public const string ClientId = "tado-web-app";

        // OAuth2 scopes
        public const string ScopeHomeUser = "home.user";
    }
}