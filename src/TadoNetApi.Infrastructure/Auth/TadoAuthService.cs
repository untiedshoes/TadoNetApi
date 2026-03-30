using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Dtos.Auth;
using TadoNetApi.Infrastructure.Extensions;

namespace TadoNetApi.Infrastructure.Auth
{
    public class TadoAuthService : ITadoAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TadoAuthService> _logger;
        private readonly TadoApiConfig _config;

        private TadoAuthResponse? _token;
        private readonly object _lock = new();

        // Async coordination (critical fix)
        private TaskCompletionSource<string> _tokenReady =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TadoAuthService(IHttpClientFactory httpClientFactory, TadoApiConfig config, ILogger<TadoAuthService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        // ============================================================
        // PUBLIC API
        // ============================================================

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (_token != null && !_token.IsExpired)
                    return _token.AccessToken;
            }

            // 🔥 Wait for initial device flow if still running
            if (!_tokenReady.Task.IsCompleted)
            {
                _logger.LogInformation("⏳ Waiting for initial authorisation...");
                return await _tokenReady.Task.WaitAsync(cancellationToken);
            }

            // 🔄 Token expired → refresh
            _logger.LogInformation("🔄 Access token expired, refreshing...");

            var refreshed = await RefreshTokenAsync(cancellationToken);
            return refreshed.AccessToken;
        }

        // ============================================================
        // DEVICE FLOW
        // ============================================================

        public async Task<DeviceCodeResponse> StartDeviceAuthorisationAsync(
            CancellationToken cancellationToken = default)
        {
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", TadoApiEndpoints.ClientId),
                new KeyValuePair<string, string>("scope", TadoApiEndpoints.ScopeHomeUser)
            });

            var client = _httpClientFactory.CreateClient("TadoAuth");
            var response = await client.PostAsync(
                TadoApiEndpoints.DeviceAuthorizeUrl,
                form,
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = JsonSerializer.Deserialize<DeviceCodeResponse>(body)
                         ?? throw new InvalidOperationException("Failed to parse device authorisation response.");

            _logger.LogInformation(
                "Device authorisation requested: user_code={UserCode}, expires_in={Expires}s",
                result.UserCode,
                result.ExpiresIn);

            return result;
        }

        public async Task<TadoAuthResponse> WaitForDeviceTokenAsync(string deviceCode, int pollingIntervalSeconds = 5, int expiresInSeconds = 300, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(expiresInSeconds))
            {
                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", TadoApiEndpoints.ClientId),
                    new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                    new KeyValuePair<string, string>("device_code", deviceCode)
                });

                var client = _httpClientFactory.CreateClient("TadoAuth");
                var response = await client.PostAsync(
                    TadoApiEndpoints.TokenUrl,
                    form,
                    cancellationToken);

                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var token = JsonSerializer.Deserialize<TadoAuthResponse>(body)
                                ?? throw new InvalidOperationException("Token response was null");

                    lock (_lock)
                    {
                        _token = token;
                    }

                    // Signal all waiting callers
                    _tokenReady.TrySetResult(token.AccessToken);

                    _logger.LogInformation("✅ Device authorised successfully!");

                    return token;
                }

                // Handle OAuth responses
                TokenErrorResponse? error = null;
                try
                {
                    error = JsonSerializer.Deserialize<TokenErrorResponse>(body);
                }
                catch { }

                if (error?.Error == "authorization_pending")
                {
                    _logger.LogInformation("⏳ Waiting for user authorisation...");
                }
                else if (error?.Error == "slow_down")
                {
                    pollingIntervalSeconds += 2;
                    _logger.LogWarning("⚠️ Told to slow down, increasing polling interval.");
                }
                else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    _logger.LogWarning("⚠️ Tado service unavailable, retrying...");
                }
                else if (error != null)
                {
                    throw new HttpRequestException(
                        $"OAuth error: {error.Error} - {error.ErrorDescription}");
                }

                await Task.Delay(TimeSpan.FromSeconds(pollingIntervalSeconds), cancellationToken);
            }

            throw new TimeoutException("Device authorisation timed out.");
        }

        // ============================================================
        // REFRESH FLOW
        // ============================================================

        private async Task<TadoAuthResponse> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            if (_token == null || string.IsNullOrEmpty(_token.RefreshToken))
                throw new InvalidOperationException("No refresh token available");

            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", TadoApiEndpoints.ClientId),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", _token.RefreshToken)
            });

            var client = _httpClientFactory.CreateClient("TadoAuth");
            var response = await client.PostAsync(
                TadoApiEndpoints.TokenUrl,
                form,
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("⚠️ Refresh failed, re-authorisation required");

                lock (_lock)
                {
                    _token = null;
                    _tokenReady = new TaskCompletionSource<string>(
                        TaskCreationOptions.RunContinuationsAsynchronously);
                }

                throw new InvalidOperationException("Re-authorisation required");
            }

            var newToken = JsonSerializer.Deserialize<TadoAuthResponse>(body)
                           ?? throw new InvalidOperationException("Failed to refresh token");

            lock (_lock)
            {
                _token = newToken;
            }

            _logger.LogInformation("🔄 Token refreshed successfully");

            return newToken;
        }
    }
}