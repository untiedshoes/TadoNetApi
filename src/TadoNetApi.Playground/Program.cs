using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Application.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var config = new TadoApiConfig
        {
            Username = Environment.GetEnvironmentVariable("TADO_USERNAME") ?? "",
            Password = Environment.GetEnvironmentVariable("TADO_PASSWORD") ?? "",
            MaxRetries = 5,
            InitialRetryDelayMs = 1000
        };

        if (string.IsNullOrEmpty(config.Username) || string.IsNullOrEmpty(config.Password))
        {
            Console.WriteLine("❌ Missing Tado credentials in environment variables.");
            return;
        }

        // Setup DI and logging
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddTadoInfrastructure(config);

        var provider = services.BuildServiceProvider();

        // Resolve services
        var authService = provider.GetRequiredService<ITadoAuthService>();
        var userService = provider.GetRequiredService<UserAppService>();
        var homeService = provider.GetRequiredService<HomeAppService>();
        var zoneService = provider.GetRequiredService<ZoneAppService>();
        var deviceService = provider.GetRequiredService<DeviceAppService>();

        var cancellationToken = CancellationToken.None;

        Console.WriteLine("🚀 Starting Tado Playground...");

        try
        {
            // 1️⃣ Start device authorization
            Console.WriteLine("🔑 Requesting device authorisation...");
            var deviceCodeInfo = await authService.StartDeviceAuthorisationAsync(cancellationToken);

            // Prefer verification_uri_complete if available, fallback to verification_uri
            var verificationUri = !string.IsNullOrEmpty(deviceCodeInfo.VerificationUriComplete)
                                  ? deviceCodeInfo.VerificationUriComplete
                                  : deviceCodeInfo.VerificationUri;

            Console.WriteLine($"📋 Please visit {verificationUri} and enter the code: {deviceCodeInfo.UserCode}");
            Console.WriteLine("⏳ Waiting for user authorisation...");

            // 2 Poll token endpoint until user approves, respecting Tado's interval & expiration
            var token = await authService.WaitForDeviceTokenAsync(
                deviceCodeInfo.DeviceCode,
                intervalSeconds: deviceCodeInfo.Interval,
                maxWaitSeconds: deviceCodeInfo.ExpiresIn,
                cancellationToken: cancellationToken);

            Console.WriteLine("✅ Device authorised successfully!");

            // 3 Call API using authenticated token
            var user = await userService.GetMeAsync(cancellationToken);
            var homeId = user.Homes?.FirstOrDefault()?.Id ?? throw new Exception("User has no associated homes.");
            Console.WriteLine($"👤 User: {user?.Name} ({user?.Email})");

            var home = await homeService.GetHomeAsync(homeId, cancellationToken);

            if (!string.IsNullOrEmpty(home?.Name))
            {
                Console.WriteLine($"🏠 Home: {home.Name} (ID: {home.Id})");
            } else {
                Console.WriteLine($"⚠️ Could not retrieve home information for User: {user?.Name} ({user?.Email})");
            }

            // 4 Get the Zones
            var zones = await zoneService.GetZonesAsync(homeId, cancellationToken);
            if (zones.Count == 0)
            {
                Console.WriteLine("⚠️ No zones found for the home.");
                return;
            } else
            {
                Console.WriteLine($"📊 {zones.Count} Zones for Home '{home.Name}' found:");
                foreach (var zone in zones)
                {
                    var state = await zoneService.GetZoneStateAsync(homeId, zone.Id, cancellationToken);
                    zone.CurrentTemperature = state.Temperature;
                    zone.Humidity = state.Humidity;
                    zone.IsHeating = state.Power == "ON";
                    Console.WriteLine($"   - Zone: {zone.Name} (ID: {zone.Id}, Type: {zone.Type}, Zone Target Temperature: {zone.CurrentTemperature}°C, Zone Current Temperature: {zone.CurrentTemperature}°C, Zone Humidity: {zone.Humidity}%, Heating: {zone.IsHeating})");
                }
            }

            Console.WriteLine("🎉 Playground complete!");
        }
        catch (TimeoutException)
        {
            Console.WriteLine("⚠️ Device authorisation timed out. Make sure you enter the code in the browser promptly.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("⚠️ An error occurred:");
            Console.WriteLine(ex.Message);
        }
    }
}