using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Application.Services;
using TadoNetApi.Infrastructure.Exceptions;

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

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddTadoInfrastructure(config);
        var provider = services.BuildServiceProvider();

        var authService = provider.GetRequiredService<ITadoAuthService>();
        var userService = provider.GetRequiredService<UserAppService>();
        var homeService = provider.GetRequiredService<HomeAppService>();
        var zoneService = provider.GetRequiredService<ZoneAppService>();
        var deviceService = provider.GetRequiredService<DeviceAppService>();

        var cancellationToken = CancellationToken.None;
        Console.WriteLine("🚀 Starting Tado Playground...");

        try
        {
            // 1️⃣ Device authorization
            Console.WriteLine("🔑 Requesting device authorisation...");
            var deviceCodeInfo = await authService.StartDeviceAuthorisationAsync(cancellationToken);
            var verificationUri = !string.IsNullOrEmpty(deviceCodeInfo.VerificationUriComplete)
                                  ? deviceCodeInfo.VerificationUriComplete
                                  : deviceCodeInfo.VerificationUri;
            Console.WriteLine($"📋 Visit {verificationUri} and enter code: {deviceCodeInfo.UserCode}");
            Console.WriteLine("⏳ Waiting for authorisation...");

            var token = await authService.WaitForDeviceTokenAsync(
                deviceCodeInfo.DeviceCode,
                intervalSeconds: deviceCodeInfo.Interval,
                maxWaitSeconds: deviceCodeInfo.ExpiresIn,
                cancellationToken: cancellationToken);
            Console.WriteLine("✅ Device authorised successfully!");

            // 2️⃣ User & Home
            var user = await userService.GetMeAsync(cancellationToken);
            if (user.Homes == null || !user.Homes.Any())
            {
                Console.WriteLine("❌ No homes found for user.");
                return;
            }
            var homeId = user.Homes.First().Id ?? throw new Exception("Home ID is null");
            var home = await homeService.GetHomeAsync((int)homeId, cancellationToken);
            var homeState = await homeService.GetHomeStateAsync((int)homeId, cancellationToken);
            Console.WriteLine($"👤 User: {user.Name} ({user.Email})");

            if (home == null)
            {
                Console.WriteLine($"⚠️ Could not retrieve home info for {user.Name}");
                return;
            }

            Console.WriteLine($"🏠 Home: {home.Name} (ID: {home.Id})");
            Console.WriteLine($"    🌐 Timezone: {home.DateTimeZone}");
            Console.WriteLine($"    🌡 Temp unit: {home.TemperatureUnit}");
            Console.WriteLine($"    🔧 Installation Completed: {home.InstallationCompleted}");
            Console.WriteLine($"    🎄 Christmas Mode: {home.ChristmasModeEnabled}");

            if (home.ContactDetails != null)
            {
                Console.WriteLine("    📇 Contact Details:");
                Console.WriteLine($"      Phone: {home.ContactDetails.Phone}");
                Console.WriteLine($"      Email: {home.ContactDetails.Email}");
            }

            if (home.Address != null)
            {
                Console.WriteLine("    🏠 Address:");
                Console.WriteLine($"      AddressLine1: {home.Address.AddressLine1}");
                Console.WriteLine($"      AddressLine2: {home.Address.AddressLine2}");
                Console.WriteLine($"      City: {home.Address.City}");
                Console.WriteLine($"      ZipCode: {home.Address.ZipCode}");
                Console.WriteLine($"      Country: {home.Address.Country}");
            }

            if (home.Geolocation != null)
            {
                Console.WriteLine($"    📍 Geo: lat {home.Geolocation.Latitude}, long {home.Geolocation.Longitude}");
            }

            if (homeState != null)
            {
                Console.WriteLine($"    🧭 Presence: {homeState.Presence}");
            }

            // 3️⃣ Zones
            var zones = await zoneService.GetZonesAsync((int)homeId, cancellationToken);
            Console.WriteLine(zones.Count == 0
                ? "⚠️ No zones found."
                : $"📊 {zones.Count} Zones found:");
            foreach (var zone in zones)
            {
                Console.WriteLine($"   - Zone: {zone.Name} (ID: {zone.Id}, Type: {zone.CurrentType})");
            }  

            foreach (var zone in zones)
            {
                var state = await zoneService.GetZoneStateAsync((int)homeId, (int)zone.Id!, cancellationToken);
                var summary = await zoneService.GetZoneSummaryAsync((int)homeId, (int)zone.Id!, cancellationToken);

                Console.WriteLine($"   - Zone: {zone.Name} (ID: {zone.Id}, Type: {zone.CurrentType})");

                // 🌡 Current Temp
                var temp = state.SensorDataPoints?.InsideTemperature?.Celsius;
                if (temp.HasValue)
                {
                    if (temp.Value >= 25) Console.ForegroundColor = ConsoleColor.Red;
                    else if (temp.Value <= 18) Console.ForegroundColor = ConsoleColor.Blue;
                    else Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine($"       🌡 Current: {temp.Value}°C");
                    Console.ResetColor();
                }

                // 💧 Humidity
                var humidity = state.SensorDataPoints?.Humidity?.Percentage;
                if (humidity.HasValue)
                    Console.WriteLine($"       💧 Humidity: {humidity.Value}%");

                // 🔥 Heating Power
                var heatingPowerPercent = state.ActivityDataPoints?.HeatingPower?.Percentage;
                if (heatingPowerPercent.HasValue)
                {
                    Console.ForegroundColor = heatingPowerPercent > 0 ? ConsoleColor.Red : ConsoleColor.Gray;
                    Console.WriteLine($"       🔥 Heating Power (actual): {heatingPowerPercent.Value}%");
                    Console.ResetColor();
                }

                if (state.Setting?.Power != null)
                {
                    Console.WriteLine($"       🔥 Setting Power: {state.Setting.Power}");
                }

                // 🎯 Target Temperature from ZoneSummary
                var targetTemp = summary?.Setting?.Temperature?.Celsius;
                if (targetTemp.HasValue)
                    Console.WriteLine($"       🎯 Target Temp: {targetTemp.Value}°C");

                // ⏱ Overlay
                if (summary?.Termination?.Type != null)
                    Console.WriteLine($"       ⏱ Overlay: {summary.Termination.Type}, Duration: {summary.Termination.DurationInSeconds}s");
            }

            // 4️⃣ Devices
            try
            {
                var devices = await deviceService.GetDevicesAsync((int)homeId, cancellationToken);
                Console.WriteLine($"📦 Devices ({devices.Count}) in Home '{home?.Name ?? "Unknown"}':");

                foreach (var device in devices)
                {
                    Console.WriteLine($"   • Device Type: {device.DeviceType}");
                    Console.WriteLine($"       Serial: {device.SerialNo}");
                    Console.WriteLine($"       Short Serial: {device.ShortSerialNo}");
                    Console.WriteLine($"       Firmware: {device.CurrentFwVersion}");
                    Console.WriteLine($"       Connection: {(device.ConnectionState?.Value == true ? "Connected" : "Disconnected")}");
                    Console.WriteLine($"       Battery: {device.BatteryState ?? "Unknown"}");
                    Console.WriteLine($"       Child Lock Enabled: {device.ChildLockEnabled?.ToString() ?? "N/A"}");
                    if (device.Characteristics?.Capabilities != null && device.Characteristics.Capabilities.Any())
                        Console.WriteLine($"       Characteristics: {string.Join(", ", device.Characteristics.Capabilities)}");
                    if (device.Duties != null && device.Duties.Any())
                        Console.WriteLine($"       Duties: {string.Join(", ", device.Duties)}");
                }
            }
            catch (TadoApiException ex)
            {
                Console.WriteLine($"⚠️ Device API Error ({ex.StatusCode}): {ex.Message}");
            }

            Console.WriteLine("🎉 Playground complete!");
        }
        catch (TimeoutException)
        {
            Console.WriteLine("⚠️ Device authorisation timed out. Enter the code promptly in the browser.");
        }
        catch (TadoApiException apiEx)
        {
            Console.WriteLine("⚠️ An error occurred:");
            Console.WriteLine(apiEx.Message);
        }
    }
}