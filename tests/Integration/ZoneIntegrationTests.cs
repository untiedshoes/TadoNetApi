using System;
using System.Globalization;
using System.Linq;
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

        #region Happy-path tests

        /// <summary>
        /// Verifies that the real device-list endpoint can be queried through the application-service layer.
        /// </summary>
        [Fact(DisplayName = "GetDeviceListAsync integration test")]
        [Trait("Category", "Integration")]
        public async Task GetDeviceListAsync_IntegrationTest()
        {
            if (!HasIntegrationEnvironment())
                return;

            using var provider = CreateProvider();
            var userService = provider.GetRequiredService<UserAppService>();
            var deviceService = provider.GetRequiredService<DeviceAppService>();
            var homeId = GetRequiredInt32("TADO_HOME_ID");

            await ValidateHomeAccessAsync(userService, homeId, CancellationToken.None);

            var entries = await deviceService.GetDeviceListAsync(homeId, CancellationToken.None);

            Assert.NotEmpty(entries);
            Assert.All(entries, entry => Assert.NotNull(entry.Device));
        }

        /// <summary>
        /// Verifies that a zone overlay can be created and then removed again through the application-service layer.
        /// Uses a dedicated heating zone and removes the overlay in cleanup.
        /// </summary>
        [Fact(DisplayName = "SetHeatingTemperatureCelsiusAsync overlay round-trip integration test")]
        [Trait("Category", "Integration")]
        public async Task SetHeatingTemperatureCelsiusAsync_OverlayRoundTrip_IntegrationTest()
        {
            if (!HasIntegrationEnvironment())
                return;

            using var provider = CreateProvider();
            var userService = provider.GetRequiredService<UserAppService>();
            var zoneService = provider.GetRequiredService<ZoneAppService>();
            var homeId = GetRequiredInt32("TADO_HOME_ID");
            var zoneId = GetRequiredInt32("TADO_HEATING_ZONE_ID");
            const double targetTemperature = 19.5;
            var overlayApplied = false;

            await ValidateHomeAccessAsync(userService, homeId, CancellationToken.None);
            await ValidateZoneAccessAsync(zoneService, homeId, zoneId, CancellationToken.None);

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
                overlayApplied = true;
            }
            finally
            {
                if (overlayApplied)
                {
                    var deleted = await zoneService.DeleteZoneOverlayAsync(homeId, zoneId, CancellationToken.None);
                    Assert.True(deleted);
                }
            }
        }

        #endregion

        #region Failure-scenario tests

        /// <summary>
        /// Verifies that GetZonesAsync throws TadoApiException when the token is invalid (401 Unauthorized).
        /// </summary>
        [Fact(DisplayName = "GetZonesAsync throws TadoApiException on invalid token")]
        [Trait("Category", "Integration")]
        public async Task GetZonesAsync_ShouldThrow_WhenTokenIsInvalid()
        {
            // Arrange — deliberately bad token, no env var gate needed
            using var provider = CreateProviderWithToken("invalid-token");
            var zoneService = provider.GetRequiredService<ZoneAppService>();
            var homeId = 12345; // any plausible ID

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TadoNetApi.Infrastructure.Exceptions.TadoApiException>(
                () => zoneService.GetZonesAsync(homeId, CancellationToken.None));

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        #endregion

        private static ServiceProvider CreateProvider()
        {
            var accessToken = Environment.GetEnvironmentVariable("TADO_ACCESS_TOKEN") ?? string.Empty;

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTadoInfrastructure(new TadoApiConfig
            {
                MaxRetries = 3,
                InitialRetryDelayMs = 250
            });
            services.AddSingleton<ITadoAuthService>(new StaticAccessTokenAuthService(accessToken));

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Constructs a DI provider using a supplied access token (for explicit token scenarios).
        /// </summary>
        /// <param name="accessToken">The access token to use for authentication.</param>
        /// <returns>A configured <see cref="ServiceProvider"/> instance.</returns>
        private static ServiceProvider CreateProviderWithToken(string accessToken)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTadoInfrastructure(new TadoApiConfig
            {
                MaxRetries = 3,
                InitialRetryDelayMs = 250
            });
            services.AddSingleton<ITadoAuthService>(new StaticAccessTokenAuthService(accessToken));
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Checks if all required environment variables for integration tests are set.
        /// </summary>
        /// <returns>True if all required environment variables are present; otherwise, false.</returns>
        private static bool HasIntegrationEnvironment()
            => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TADO_ACCESS_TOKEN"))
                && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TADO_HOME_ID"))
                && !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TADO_HEATING_ZONE_ID"));

        /// <summary>
        /// Validates that the supplied access token can access the specified home.
        /// Throws if the home is not accessible.
        /// </summary>
        /// <param name="userService">The user service to query homes.</param>
        /// <param name="homeId">The home ID to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        private static async Task ValidateHomeAccessAsync(UserAppService userService, int homeId, CancellationToken cancellationToken)
        {
            var me = await userService.GetMeAsync(cancellationToken);
            var homes = me?.Homes ?? [];

            if (homes.Any(home => home.Id == homeId))
                return;

            var visibleHomes = homes.Length == 0
                ? "none"
                : string.Join(", ", homes.Select(home => $"{home.Name ?? "Unnamed Home"} ({home.Id})"));

            throw new InvalidOperationException(
                $"TADO_HOME_ID={homeId} is not accessible for the supplied TADO_ACCESS_TOKEN. Homes visible to the token: {visibleHomes}.");
        }

        /// <summary>
        /// Validates that the supplied access token can access the specified zone in the given home.
        /// Throws if the zone is not accessible.
        /// </summary>
        /// <param name="zoneService">The zone service to query zones.</param>
        /// <param name="homeId">The home ID containing the zone.</param>
        /// <param name="zoneId">The zone ID to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        private static async Task ValidateZoneAccessAsync(ZoneAppService zoneService, int homeId, int zoneId, CancellationToken cancellationToken)
        {
            var zones = await zoneService.GetZonesAsync(homeId, cancellationToken);

            if (zones.Any(zone => zone.Id == zoneId))
                return;

            var visibleZones = zones.Count == 0
                ? "none"
                : string.Join(", ", zones.Select(zone => $"{zone.Name ?? "Unnamed Zone"} ({zone.Id})"));

            throw new InvalidOperationException(
                $"TADO_HEATING_ZONE_ID={zoneId} is not accessible within TADO_HOME_ID={homeId}. Zones visible to the token: {visibleZones}.");
        }

        /// <summary>
        /// Reads and parses an integer environment variable, throwing if missing or invalid.
        /// </summary>
        /// <param name="variableName">The environment variable name.</param>
        /// <returns>The parsed integer value.</returns>
        private static int GetRequiredInt32(string variableName)
        {
            var rawValue = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrWhiteSpace(rawValue))
                throw new InvalidOperationException($"Environment variable {variableName} must be set.");

            if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue))
                throw new InvalidOperationException($"Environment variable {variableName} must be an integer.");

            return parsedValue;
        }

        /// <summary>
        /// Simple ITadoAuthService implementation for integration tests using a static access token.
        /// </summary>
        private sealed class StaticAccessTokenAuthService : ITadoAuthService
        {
            private readonly string _accessToken;

            /// <summary>
            /// Initializes a new instance with the provided access token.
            /// </summary>
            /// <param name="accessToken">The static access token to use.</param>
            public StaticAccessTokenAuthService(string accessToken)
            {
                _accessToken = accessToken;
            }

            public Task<DeviceCodeResponse> StartDeviceAuthorisationAsync(CancellationToken cancellationToken = default)
                => throw new NotSupportedException("Static access-token integration tests do not use the device flow.");

            /// <summary>
            /// Not supported for static token tests.
            /// </summary>
            public Task<TadoAuthResponse> WaitForDeviceTokenAsync(string deviceCode, int intervalSeconds, int maxWaitSeconds, CancellationToken cancellationToken = default)
                => throw new NotSupportedException("Static access-token integration tests do not use the device flow.");

            /// <summary>
            /// Returns the static access token, or throws if not set.
            /// </summary>
            public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
            {
                if (string.IsNullOrWhiteSpace(_accessToken))
                    throw new InvalidOperationException("TADO_ACCESS_TOKEN must be set to run integration tests.");

                return Task.FromResult(_accessToken);
            }
        }
    }
}