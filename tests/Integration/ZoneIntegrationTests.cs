using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TadoNetApi.Application.Services;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Dtos.Auth;
using TadoNetApi.Infrastructure.Extensions;
using Xunit;

namespace TadoNetApi.Tests.Integration
{
    /// <summary>
    /// Manual integration tests that exercise real Tado API flows when an access token and IDs are provided.
    /// </summary>
    public class DeviceAndZoneIntegrationTests
    {
        private const string SkipMessage = "Run manually with TADO_ACCESS_TOKEN, TADO_HOME_ID, and TADO_HEATING_ZONE_ID set to a safe test home/zone.";

        /// <summary>
        /// Verifies that the real device-list endpoint can be queried through the application-service layer.
        /// </summary>
        [Fact(DisplayName = "GetDeviceListAsync integration test", Skip = SkipMessage)]
        [Trait("Category", "Integration")]
        public async Task GetDeviceListAsync_IntegrationTest()
        {
            using var provider = CreateProvider();
            var deviceService = provider.GetRequiredService<DeviceAppService>();
            var homeId = GetRequiredInt32("TADO_HOME_ID");

            var entries = await deviceService.GetDeviceListAsync(homeId, CancellationToken.None);

            Assert.NotEmpty(entries);
            Assert.All(entries, entry => Assert.NotNull(entry.Device));
        }

        /// <summary>
        /// Verifies that a zone overlay can be created and then removed again through the application-service layer.
        /// Uses a dedicated heating zone and removes the overlay in cleanup.
        /// </summary>
        [Fact(DisplayName = "SetHeatingTemperatureCelsiusAsync overlay round-trip integration test", Skip = SkipMessage)]
        [Trait("Category", "Integration")]
        public async Task SetHeatingTemperatureCelsiusAsync_OverlayRoundTrip_IntegrationTest()
        {
            using var provider = CreateProvider();
            var zoneService = provider.GetRequiredService<ZoneAppService>();
            var homeId = GetRequiredInt32("TADO_HOME_ID");
            var zoneId = GetRequiredInt32("TADO_HEATING_ZONE_ID");
            const double targetTemperature = 19.5;

            try
            {
                var summary = await zoneService.SetHeatingTemperatureCelsiusAsync(
                    homeId,
                    zoneId,
                    targetTemperature,
                    CancellationToken.None);

                Assert.NotNull(summary);
                Assert.NotNull(summary!.Setting);
                Assert.NotNull(summary.Setting!.Temperature);
                Assert.Equal(targetTemperature, summary.Setting.Temperature!.Celsius);
            }
            finally
            {
                var deleted = await zoneService.DeleteZoneOverlayAsync(homeId, zoneId, CancellationToken.None);
                Assert.True(deleted);
            }
        }

        private static ServiceProvider CreateProvider()
        {
            var accessToken = Environment.GetEnvironmentVariable("TADO_ACCESS_TOKEN") ?? string.Empty;

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTadoInfrastructure(new TadoApiConfig
            {
                Username = string.Empty,
                Password = string.Empty,
                MaxRetries = 3,
                InitialRetryDelayMs = 250
            });
            services.AddSingleton<ITadoAuthService>(new StaticAccessTokenAuthService(accessToken));

            return services.BuildServiceProvider();
        }

        private static int GetRequiredInt32(string variableName)
        {
            var rawValue = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrWhiteSpace(rawValue))
                throw new InvalidOperationException($"Environment variable {variableName} must be set.");

            if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue))
                throw new InvalidOperationException($"Environment variable {variableName} must be an integer.");

            return parsedValue;
        }

        private sealed class StaticAccessTokenAuthService : ITadoAuthService
        {
            private readonly string _accessToken;

            public StaticAccessTokenAuthService(string accessToken)
            {
                _accessToken = accessToken;
            }

            public Task<DeviceCodeResponse> StartDeviceAuthorisationAsync(CancellationToken cancellationToken = default)
                => throw new NotSupportedException("Static access-token integration tests do not use the device flow.");

            public Task<TadoAuthResponse> WaitForDeviceTokenAsync(string deviceCode, int intervalSeconds, int maxWaitSeconds, CancellationToken cancellationToken = default)
                => throw new NotSupportedException("Static access-token integration tests do not use the device flow.");

            public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
            {
                if (string.IsNullOrWhiteSpace(_accessToken))
                    throw new InvalidOperationException("TADO_ACCESS_TOKEN must be set to run integration tests.");

                return Task.FromResult(_accessToken);
            }
        }
    }
}