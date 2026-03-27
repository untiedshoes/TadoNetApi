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
        public const string ClientId = "1bb50063-6b0c-4d11-bd99-387f4a91cc46";
        public const string ScopeHomeUser = "home.user";
    }
}