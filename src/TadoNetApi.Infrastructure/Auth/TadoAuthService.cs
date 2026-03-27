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

namespace TadoNetApi.Infrastructure.Auth
{
    public class TadoAuthService : ITadoAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly TadoApiConfig _config;
        private readonly ILogger<TadoAuthService> _logger;

        private TadoAuthResponse? _token;
        private readonly object _lock = new();

        public TadoAuthService(HttpClient httpClient, TadoApiConfig config, ILogger<TadoAuthService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Returns cached token if valid.
        /// </summary>
        public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (_token != null && !_token.IsExpired)
                    return Task.FromResult(_token.AccessToken);
            }

            throw new InvalidOperationException("Access token unavailable. Start device flow first.");
        }

        /// <summary>
        /// Step 1: Request device authorisation (user_code + device_code)
        /// </summary>
        public async Task<DeviceCodeResponse> StartDeviceAuthorisationAsync(CancellationToken cancellationToken = default)
        {
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", TadoApiEndpoints.ClientId),
                new KeyValuePair<string, string>("scope", TadoApiEndpoints.ScopeHomeUser)
            });

            var response = await _httpClient.PostAsync(TadoApiEndpoints.DeviceAuthorizeUrl, formData, cancellationToken);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var deviceCode = JsonSerializer.Deserialize<DeviceCodeResponse>(body)
                             ?? throw new InvalidOperationException("Failed to deserialize device authorisation response.");

            _logger.LogInformation("Device authorisation requested: user_code={UserCode}, expires_in={Expires}s",
                deviceCode.UserCode, deviceCode.ExpiresIn);

            return deviceCode;
        }

        /// <summary>
        /// Step 2: Poll /token endpoint until user approves device.
        /// Implements interval from Tado + exponential backoff.
        /// </summary>
        public async Task<TadoAuthResponse> WaitForDeviceTokenAsync(
    string deviceCode,
    int pollingIntervalSeconds = 5,
    int expiresInSeconds = 300,
    CancellationToken cancellationToken = default)
        {
            int attempt = 0;
            int delayMs = pollingIntervalSeconds * 1000;
            var maxWaitMs = expiresInSeconds * 1000;
            var startTime = DateTime.UtcNow;

            while ((DateTime.UtcNow - startTime).TotalMilliseconds < maxWaitMs)
            {
                attempt++;

                try
                {
                    var formData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", TadoApiEndpoints.ClientId),
                        new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                        new KeyValuePair<string, string>("device_code", deviceCode)
                    });

                    var response = await _httpClient.PostAsync(TadoApiEndpoints.TokenUrl, formData, cancellationToken);
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var token = JsonSerializer.Deserialize<TadoAuthResponse>(body)
                                    ?? throw new InvalidOperationException("Token response was null");

                        lock (_lock) { _token = token; }
                        return token;
                    }

                    // Temporary server issues
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable || (int)response.StatusCode == 429)
                    {
                        _logger.LogWarning("Transient server error {StatusCode}, retry attempt {Attempt}. Waiting {Delay}ms...",
                            response.StatusCode, attempt, delayMs);

                        await Task.Delay(delayMs, cancellationToken);
                        delayMs = Math.Min(delayMs * 2, 30_000); // max 30s
                        continue;
                    }

                    var error = JsonSerializer.Deserialize<TokenErrorResponse>(body);
                    if (error?.Error == "authorization_pending")
                    {
                        _logger.LogInformation("Waiting for user authorisation...");
                        await Task.Delay(delayMs, cancellationToken);
                        continue;
                    }

                    throw new HttpRequestException($"Failed to get token. Status={response.StatusCode}, Body={body}");
                }
                catch (TaskCanceledException)
                {
                    throw new TimeoutException("Token request timed out.");
                }
            }

            throw new TimeoutException("Device authorisation timed out. User did not approve the device in time.");
        }
    }
}