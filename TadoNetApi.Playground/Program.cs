using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
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
            MaxRetries = 5,
            InitialRetryDelayMs = 1000
        };

        var enableVerboseHttpLogs = string.Equals(
            Environment.GetEnvironmentVariable("TADO_VERBOSE_HTTP_LOGS"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();

            if (!enableVerboseHttpLogs)
            {
                builder.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
                builder.AddFilter("System.Net.Http.HttpClient.ITadoHttpClient", LogLevel.Warning);
                builder.AddFilter("TadoNetApi.Infrastructure.Http.TadoHttpClient", LogLevel.Warning);
                builder.AddFilter("TadoNetApi.Infrastructure.Auth.TadoAuthService", LogLevel.Warning);
            }
        });
        services.AddTadoInfrastructure(config);
        var provider = services.BuildServiceProvider();

        var authService = provider.GetRequiredService<ITadoAuthService>();
        var userService = provider.GetRequiredService<UserAppService>();
        var homeService = provider.GetRequiredService<HomeAppService>();
        var zoneService = provider.GetRequiredService<ZoneAppService>();
        var deviceService = provider.GetRequiredService<DeviceAppService>();
        var weatherService = provider.GetRequiredService<WeatherAppService>();
        var bridgeService = provider.GetRequiredService<BridgeAppService>();
        var boilerByBridgeService = provider.GetRequiredService<BoilerByBridgeAppService>();

        var configuredBridgeId = Environment.GetEnvironmentVariable("TADO_BRIDGE_ID");
        var bridgeAuthKey = Environment.GetEnvironmentVariable("TADO_BRIDGE_AUTH_KEY");

        var cancellationToken = CancellationToken.None;
        Console.WriteLine("🚀 Starting Tado Playground...");

        try
        {
            // 1️⃣ Device authorization
            WriteSectionHeader("🔑 Device Authorisation");
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
            if (user?.Homes == null || !user.Homes.Any())
            {
                Console.WriteLine("❌ No homes found for user.");
                return;
            }
            var homeId = user.Homes.First().Id ?? throw new Exception("Home ID is null");
            var home = await homeService.GetHomeAsync((int)homeId, cancellationToken);
            var homeState = await homeService.GetHomeStateAsync((int)homeId, cancellationToken);
            WriteSectionHeader("👤 User & Home");
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
            Console.WriteLine($"    🗓 Simple Smart Schedule: {home.SimpleSmartScheduleEnabled}");
            Console.WriteLine($"    🚶‍♂️ Away Radius: {home.AwayRadiusInMeters} meters");


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

            // 3️⃣ Installations
            try
            {
                WriteSectionHeader("🛠 Installations");
                var installations = await homeService.GetInstallationsAsync((int)homeId, cancellationToken);

                Console.WriteLine(installations.Count == 0
                    ? "🛠 No installations found. This is still a successful response path and is expected for homes without AC installations."
                    : $"🛠 Installations ({installations.Count}):");

                foreach (var installation in installations)
                {
                    Console.WriteLine($"   • Installation {installation.Id} ({installation.CurrentType ?? "Unknown Type"})");
                    Console.WriteLine($"       State: {installation.State ?? "Unknown"}");
                    Console.WriteLine($"       Revision: {installation.Revision?.ToString() ?? "Unknown"}");
                    Console.WriteLine($"       Devices: {installation.Devices.Length}");

                    if (installation.Id.HasValue)
                    {
                        var installationDetails = await homeService.GetInstallationAsync((int)homeId, (int)installation.Id.Value, cancellationToken);

                        if (installationDetails != null)
                        {
                            Console.WriteLine($"       Detail State: {installationDetails.State ?? "Unknown"}");

                            if (installationDetails.Devices.Length > 0)
                            {
                                foreach (var installationDevice in installationDetails.Devices)
                                {
                                    var deviceName = !string.IsNullOrWhiteSpace(installationDevice.DeviceTypeName)
                                        ? installationDevice.DeviceTypeName
                                        : installationDevice.DeviceType ?? "Unknown Device";
                                    var deviceIdentifier = !string.IsNullOrWhiteSpace(installationDevice.ShortSerialNo)
                                        ? installationDevice.ShortSerialNo
                                        : installationDevice.SerialNo ?? "Unknown Serial";

                                    Console.WriteLine($"         - {deviceName} ({deviceIdentifier})");
                                }
                            }
                        }
                    }
                }
            }
            catch (TadoApiException ex)
            {
                Console.WriteLine($"⚠️ Installation Error ({ex.StatusCode}): {ex.Message}");
            }

            // 5️⃣ Zones
            var zones = await zoneService.GetZonesAsync((int)homeId, cancellationToken);
            var zoneByDeviceShortSerial = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var measuredZoneByDeviceSerial = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var zoneNameById = zones
                .Where(z => z.Id.HasValue)
                .ToDictionary(z => z.Id!.Value, z => z.Name ?? "Unknown Zone");

            WriteSectionHeader("🧩 Zones");

            Console.WriteLine(zones.Count == 0
                ? "⚠️ No zones found."
                : $"📊 {zones.Count} Zones found:");

            if (zones.Count > 0)
            {
                Console.WriteLine($"    {string.Join(", ", zones.Select(zone => zone.Name ?? $"Zone {zone.Id}"))}");
                Console.WriteLine();
                Console.WriteLine("🧩 Zone Details:");
            }

            try
            {
                var deviceListEntries = await deviceService.GetDeviceListAsync((int)homeId, cancellationToken);

                foreach (var entry in deviceListEntries)
                {
                    if (entry.Device == null || !entry.ZoneId.HasValue)
                        continue;

                    var zoneName = zoneNameById.TryGetValue(entry.ZoneId.Value, out var resolvedZoneName)
                        ? resolvedZoneName
                        : "Unknown Zone";

                    if (!string.IsNullOrWhiteSpace(entry.Device.ShortSerialNo))
                        zoneByDeviceShortSerial[entry.Device.ShortSerialNo] = zoneName;

                    if (!string.IsNullOrWhiteSpace(entry.Device.SerialNo))
                        zoneByDeviceShortSerial[entry.Device.SerialNo] = zoneName;
                }
            }
            catch (TadoApiException ex)
            {
                Console.WriteLine($"⚠️ Device-list zone mapping unavailable ({ex.StatusCode}). Continuing with best effort.");
            }


            foreach (var zone in zones)
            {
                var state = await zoneService.GetZoneStateAsync((int)homeId, (int)zone.Id!, cancellationToken);
                var summary = await zoneService.GetZoneSummaryAsync((int)homeId, (int)zone.Id!, cancellationToken);

                Console.WriteLine();
                Console.WriteLine($"   • {zone.Name} (ID: {zone.Id}, Type: {zone.CurrentType})");

                try
                {
                    var measuringDevice = await deviceService.GetZoneMeasuringDeviceAsync((int)homeId, (int)zone.Id!, cancellationToken);
                    var measuringDeviceName = !string.IsNullOrWhiteSpace(measuringDevice.DeviceTypeName)
                        ? measuringDevice.DeviceTypeName
                        : measuringDevice.DeviceType ?? "Unknown Device";
                    var measuringDeviceIdentifier = !string.IsNullOrWhiteSpace(measuringDevice.ShortSerialNo)
                        ? measuringDevice.ShortSerialNo
                        : measuringDevice.SerialNo ?? "Unknown Serial";

                    if (!string.IsNullOrWhiteSpace(measuringDevice.ShortSerialNo))
                        measuredZoneByDeviceSerial[measuringDevice.ShortSerialNo] = zone.Name ?? "Unknown Zone";

                    if (!string.IsNullOrWhiteSpace(measuringDevice.SerialNo))
                        measuredZoneByDeviceSerial[measuringDevice.SerialNo] = zone.Name ?? "Unknown Zone";

                    Console.WriteLine($"       🌡 Measuring Device: {measuringDeviceName} ({measuringDeviceIdentifier})");
                }
                catch (TadoApiException ex)
                {
                    Console.WriteLine($"       ⚠️ Measuring Device Error ({ex.StatusCode}): {ex.Message}");
                }

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

                try
                {
                    await WriteZoneTimetableAsync(zoneService, (int)homeId, (int)zone.Id!, cancellationToken);
                }
                catch (TadoApiException ex)
                {
                    Console.WriteLine($"       ⚠️ Timetable Error ({ex.StatusCode}): {ex.Message}");
                }

                // 🔧 Capabilities
                try
                {
                    var capabilities = await zoneService.GetZoneCapabilitiesAsync((int)homeId, (int)zone.Id!, cancellationToken);
                    if (capabilities.Any())
                    {
                        Console.WriteLine($"       🔧 Capabilities:");
                        foreach (var cap in capabilities)
                        {
                            Console.WriteLine($"         - {cap.PurpleType}");
                            if (cap.Temperatures?.Celsius != null)
                            {
                                Console.WriteLine($"           Celsius: {cap.Temperatures.Celsius.Min}° - {cap.Temperatures.Celsius.Max}° (step: {cap.Temperatures.Celsius.Step})");
                            }
                            if (cap.Temperatures?.Fahrenheit != null)
                            {
                                Console.WriteLine($"           Fahrenheit: {cap.Temperatures.Fahrenheit.Min}° - {cap.Temperatures.Fahrenheit.Max}° (step: {cap.Temperatures.Fahrenheit.Step})");
                            }
                        }
                    }
                }
                catch (TadoApiException ex)
                {
                    Console.WriteLine($"       ⚠️ Capabilities Error ({ex.StatusCode}): {ex.Message}");
                }
            }

            // 6️⃣ Devices
            string? detectedBridgeId = null;
            string? sayHiDeviceId = null;
            string? sayHiDeviceSerialNo = null;
            string sayHiDeviceName = "Unknown Device";
            string sayHiZoneName = "Unknown Zone";
            try
            {
                WriteSectionHeader("📦 Devices");
                var devices = await deviceService.GetDevicesAsync((int)homeId, cancellationToken);
                Console.WriteLine($"📦 Devices ({devices.Count}) in Home '{home?.Name ?? "Unknown"}':");

                foreach (var device in devices)
                {
                    Console.WriteLine($"   • Device Type: {device.DeviceType}");
                    if (!string.IsNullOrWhiteSpace(device.DeviceTypeName))
                        Console.WriteLine($"       Device Name: {device.DeviceTypeName} ({device.DeviceType})");
                    Console.WriteLine($"       Serial: {device.SerialNo}");
                    Console.WriteLine($"       Short Serial: {device.ShortSerialNo}");
                    Console.WriteLine($"       Firmware: {device.CurrentFwVersion}");
                    Console.WriteLine($"       Connection: {(device.ConnectionState?.Value == true ? "Connected" : "Disconnected")}");
                    Console.WriteLine($"       Battery: {device.BatteryState ?? "Unknown"}");
                    Console.WriteLine($"       Child Lock Enabled: {device.ChildLockEnabled?.ToString() ?? "N/A"}");
                    if (device.Duties != null && device.Duties.Any())
                        Console.WriteLine($"       Duties: {string.Join(", ", device.Duties)}");

                    if (detectedBridgeId == null
                        && string.Equals(device.DeviceType, "IB01", StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(device.SerialNo))
                    {
                        detectedBridgeId = device.SerialNo;
                    }

                    var measuredZoneName = !string.IsNullOrWhiteSpace(device.ShortSerialNo)
                        && measuredZoneByDeviceSerial.TryGetValue(device.ShortSerialNo, out var measuredZoneByShortSerial)
                            ? measuredZoneByShortSerial
                            : !string.IsNullOrWhiteSpace(device.SerialNo)
                                && measuredZoneByDeviceSerial.TryGetValue(device.SerialNo, out var measuredZoneBySerial)
                                    ? measuredZoneBySerial
                                    : null;

                    if (!string.IsNullOrWhiteSpace(measuredZoneName))
                        Console.WriteLine($"       🌡 Measures Temperature For: {measuredZoneName}");

                    var canUseForSayHi = !string.IsNullOrWhiteSpace(device.ShortSerialNo)
                        && !device.ShortSerialNo.StartsWith("IB", StringComparison.OrdinalIgnoreCase)
                        && !device.ShortSerialNo.StartsWith("BP", StringComparison.OrdinalIgnoreCase);

                    if (sayHiDeviceId == null && canUseForSayHi)
                    {
                        sayHiDeviceId = device.ShortSerialNo;
                        sayHiDeviceSerialNo = device.SerialNo;
                        sayHiDeviceName = !string.IsNullOrWhiteSpace(device.DeviceTypeName)
                            ? $"{device.DeviceTypeName} ({device.DeviceType})"
                            : device.DeviceType ?? device.SerialNo ?? "Unknown Device";

                        if (!string.IsNullOrWhiteSpace(sayHiDeviceId)
                            && zoneByDeviceShortSerial.TryGetValue(sayHiDeviceId, out var matchedZoneByShortSerial))
                        {
                            sayHiZoneName = matchedZoneByShortSerial;
                        }
                        else if (!string.IsNullOrWhiteSpace(sayHiDeviceSerialNo)
                            && zoneByDeviceShortSerial.TryGetValue(sayHiDeviceSerialNo, out var matchedZoneBySerial))
                        {
                            sayHiZoneName = matchedZoneBySerial;
                        }
                    }
                }
            }
            catch (TadoApiException ex)
            {
                Console.WriteLine($"⚠️ Device API Error ({ex.StatusCode}): {ex.Message}");
            }

            WriteSectionHeader("🔌 Bridge Diagnostics");
            Console.WriteLine("    BridgeId is the Internet Bridge serial number.");
            Console.WriteLine("    The playground auto-detects it from the IB01 device Serial value shown above.");
            Console.WriteLine("    You can still override it with TADO_BRIDGE_ID if needed.");
            Console.WriteLine("    Provide the printed bridge auth key either by setting TADO_BRIDGE_AUTH_KEY before launch");
            Console.WriteLine("    or by entering it when prompted below.");

            var effectiveBridgeId = !string.IsNullOrWhiteSpace(configuredBridgeId)
                ? configuredBridgeId
                : detectedBridgeId;

            if (!string.IsNullOrWhiteSpace(effectiveBridgeId))
            {
                var bridgeIdSource = !string.IsNullOrWhiteSpace(configuredBridgeId)
                    ? "TADO_BRIDGE_ID"
                    : "auto-detected IB01 device";
                Console.WriteLine($"    Using BridgeId: {effectiveBridgeId} ({bridgeIdSource})");
            }
            else
            {
                Console.WriteLine("    ℹ️ Bridge diagnostics skipped because no IB01 bridge device was detected and TADO_BRIDGE_ID was not set.");
            }

            if (string.IsNullOrWhiteSpace(bridgeAuthKey) && !string.IsNullOrWhiteSpace(effectiveBridgeId))
            {
                Console.Write("    Enter TADO_BRIDGE_AUTH_KEY (printed on the Internet Bridge), or press Enter to skip: ");
                var promptedBridgeAuthKey = Console.ReadLine();
                bridgeAuthKey = string.IsNullOrWhiteSpace(promptedBridgeAuthKey)
                    ? null
                    : promptedBridgeAuthKey.Trim();
            }

            if (string.IsNullOrWhiteSpace(effectiveBridgeId) || string.IsNullOrWhiteSpace(bridgeAuthKey))
            {
                if (!string.IsNullOrWhiteSpace(effectiveBridgeId))
                    Console.WriteLine("    ℹ️ Bridge diagnostics skipped because no bridge auth key was provided.");
            }
            else
            {
                await WriteBridgeDiagnosticsAsync(
                    bridgeService,
                    boilerByBridgeService,
                    effectiveBridgeId,
                    bridgeAuthKey,
                    cancellationToken);
            }

            // 7️⃣ Mobile Devices
            try
            {
                WriteSectionHeader("📱 Mobile Devices");
                var mobileDevices = await deviceService.GetMobileDevicesAsync((int)homeId, cancellationToken);
                Console.WriteLine($"📱 Mobile Devices ({mobileDevices.Count}) in Home '{home?.Name ?? "Unknown"}':");

                foreach (var md in mobileDevices)
                {
                    Console.WriteLine($"   • {md.Name} (ID: {md.Id})");

                    if (md.Settings != null)
                        Console.WriteLine($"       Geo Tracking: {(md.Settings.GeoTrackingEnabled == true ? "Enabled" : "Disabled")}");

                    if (md.Location != null)
                    {
                        Console.WriteLine($"       At Home: {(md.Location.AtHome == true ? "Yes" : "No")}");
                        Console.WriteLine($"       Location Stale: {(md.Location.Stale == true ? "Yes" : "No")}");
                        if (md.Location.RelativeDistanceFromHomeFence.HasValue)
                            Console.WriteLine($"       Distance from Fence: {md.Location.RelativeDistanceFromHomeFence.Value:0.00}");
                    }

                    if (md.MobileDeviceDetails != null)
                    {
                        Console.WriteLine($"       Platform: {md.MobileDeviceDetails.Platform}");
                        Console.WriteLine($"       OS Version: {md.MobileDeviceDetails.OsVersion}");
                        Console.WriteLine($"       Model: {md.MobileDeviceDetails.Model}");
                    }

                    if (md.Id.HasValue)
                    {
                        try
                        {
                            var settings = await deviceService.GetMobileDeviceSettingsAsync((int)homeId, (int)md.Id.Value, cancellationToken);
                            Console.WriteLine($"       Settings (API): Geo Tracking = {(settings.GeoTrackingEnabled == true ? "Enabled" : "Disabled")}");
                        }
                        catch (TadoApiException ex)
                        {
                            Console.WriteLine($"       ⚠️ Settings Error ({ex.StatusCode}): {ex.Message}");
                        }
                    }
                }
            }
            catch (TadoApiException ex)
            {
                Console.WriteLine($"⚠️ Mobile Device API Error ({ex.StatusCode}): {ex.Message}");
            }

            // 8️⃣ Weather
            try
            {
                WriteSectionHeader("☁️ Weather");
                var weather = await weatherService.GetWeatherAsync((int)homeId, cancellationToken);                var state = weather.WeatherState?.Value ?? weather.WeatherState?.CurrentType ?? "Unknown";
                var temperatureC = weather.OutsideTemperature?.Celsius?.ToString("0.0") ?? "N/A";
                var temperatureF = weather.OutsideTemperature?.Fahrenheit?.ToString("0.0") ?? "N/A";
                var solarType = weather.SolarIntensity?.CurrentType ?? "Unknown";
                var solarPercentage = weather.SolarIntensity?.Percentage?.ToString("0.0") ?? "N/A";

                Console.WriteLine($"☁️ Weather for home '{home?.Name ?? "Unknown"}': {state}, {temperatureC}°C ({temperatureF}°F)");
                Console.WriteLine($"    ☀️ Solar Intensity: {solarType}, {solarPercentage}%");
            }
            catch (TadoApiException ex)
            {
                Console.WriteLine($"⚠️ Weather API Error ({ex.StatusCode}): {ex.Message}");
            }

            // 9️⃣ Simple Command Example
            if (!string.IsNullOrWhiteSpace(sayHiDeviceId))
            {
                WriteSectionHeader("👋 Simple Command Example");
                var sayHiResult = await deviceService.SayHiAsync(sayHiDeviceId, cancellationToken);
                Console.WriteLine($"👋 SayHiAsync (Zone: {sayHiZoneName}, Device: {sayHiDeviceName}, ID: {sayHiDeviceId}): {sayHiResult}");
            }
            else
            {
                WriteSectionHeader("👋 Simple Command Example");
                Console.WriteLine("ℹ️ SayHiAsync skipped (no eligible device found; short serial must not start with IB or BP).");
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

    private static void WriteSectionHeader(string title)
    {
        const string divider = "======================";
        Console.WriteLine();
        Console.WriteLine(divider);
        Console.WriteLine(title + ":");
        Console.WriteLine(divider);
    }

    private static async Task WriteZoneTimetableAsync(
        ZoneAppService zoneService,
        int homeId,
        int zoneId,
        CancellationToken cancellationToken)
    {
        var activeTimetable = await zoneService.GetActiveTimetableTypeAsync(homeId, zoneId, cancellationToken);

        if (!activeTimetable.Id.HasValue)
        {
            Console.WriteLine("       🗓 Schedule Days: unavailable");
            return;
        }

        var blocks = await zoneService.GetZoneTimetableBlocksAsync(homeId, zoneId, activeTimetable.Id.Value, cancellationToken);
        var groupedBlocks = blocks
            .GroupBy(block => NormalizeDayType(block.DayType))
            .OrderBy(group => GetDaySortOrder(group.Key, activeTimetable.Type))
            .ToList();

        var scheduleDays = groupedBlocks.Count > 0
            ? string.Join(", ", groupedBlocks.Select(group => ToDisplayDayLabel(group.Key, activeTimetable.Type)))
            : GetDefaultScheduleDays(activeTimetable.Type);

        Console.WriteLine($"       🗓 Schedule Days: {scheduleDays}");

        if (groupedBlocks.Count == 0)
        {
            Console.WriteLine("       📋 Schedule: no timetable blocks returned.");
            return;
        }

        Console.WriteLine("       📋 Schedule Table:");
        Console.WriteLine($"         {PadCell("Day", 12)} {PadCell("Start", 6)} {PadCell("End", 6)} {PadCell("Power", 5)} {PadCell("Target", 8)} Setting");

        for (var groupIndex = 0; groupIndex < groupedBlocks.Count; groupIndex++)
        {
            var group = groupedBlocks[groupIndex];

            if (groupIndex > 0)
                Console.WriteLine();

            foreach (var block in group.OrderBy(block => block.Start, StringComparer.Ordinal))
            {
                Console.WriteLine(
                    $"         {PadCell(ToDisplayDayLabel(group.Key, activeTimetable.Type), 12)} {PadCell(block.Start, 6)} {PadCell(block.End, 6)} {PadCell(block.Setting?.Power?.ToString(), 5)} {PadCell(FormatTarget(block.Setting), 8)} {FormatSettingSummary(block)}");
            }
        }
    }

        private static async Task WriteBridgeDiagnosticsAsync(
            BridgeAppService bridgeService,
            BoilerByBridgeAppService boilerByBridgeService,
            string bridgeId,
            string bridgeAuthKey,
            CancellationToken cancellationToken)
        {
            try
            {
                var bridge = await bridgeService.GetBridgeAsync(bridgeId, bridgeAuthKey, cancellationToken);

                if (bridge == null)
                {
                    Console.WriteLine("    ⚠️ Bridge lookup returned no payload.");
                    return;
                }

                Console.WriteLine($"    Bridge ID: {bridgeId}");
                Console.WriteLine($"    Linked Home ID: {bridge.HomeId?.ToString() ?? "Unknown"}");

                var boilerInfo = await boilerByBridgeService.GetBoilerInfoAsync(bridgeId, bridgeAuthKey, cancellationToken);
                if (boilerInfo != null)
                {
                    Console.WriteLine($"    Boiler Present: {FormatBoolean(boilerInfo.BoilerPresent)}");
                    Console.WriteLine($"    Boiler ID: {boilerInfo.BoilerId?.ToString() ?? "Unknown"}");
                }

                var boilerMaxOutputTemperature = await boilerByBridgeService.GetBoilerMaxOutputTemperatureAsync(bridgeId, bridgeAuthKey, cancellationToken);
                if (boilerMaxOutputTemperature?.BoilerMaxOutputTemperatureInCelsius.HasValue == true)
                {
                    Console.WriteLine($"    Boiler Max Output Temperature: {boilerMaxOutputTemperature.BoilerMaxOutputTemperatureInCelsius:0.0}°C");
                }

                var wiringState = await boilerByBridgeService.GetBoilerWiringInstallationStateAsync(bridgeId, bridgeAuthKey, cancellationToken);
                if (wiringState != null)
                {
                    Console.WriteLine($"    Wiring State: {wiringState.State ?? "Unknown"}");
                    Console.WriteLine($"    Bridge Connected: {FormatBoolean(wiringState.BridgeConnected)}");
                    Console.WriteLine($"    Hot Water Zone Present: {FormatBoolean(wiringState.HotWaterZonePresent)}");

                    if (wiringState.DeviceWiredToBoiler != null)
                    {
                        Console.WriteLine($"    Wired Device: {wiringState.DeviceWiredToBoiler.Type ?? "Unknown"} ({wiringState.DeviceWiredToBoiler.SerialNo ?? "Unknown Serial"})");
                        Console.WriteLine($"      Therm Interface: {wiringState.DeviceWiredToBoiler.ThermInterfaceType ?? "Unknown"}");
                        Console.WriteLine($"      Connected: {FormatBoolean(wiringState.DeviceWiredToBoiler.Connected)}");
                    }

                    if (wiringState.Boiler?.OutputTemperature?.Celsius.HasValue == true)
                    {
                        Console.WriteLine($"    Boiler Output Temperature: {wiringState.Boiler.OutputTemperature.Celsius:0.0}°C");
                    }
                }
            }
            catch (TadoApiException ex)
            {
                Console.WriteLine($"    ⚠️ Bridge Diagnostics Error ({ex.StatusCode}): {ex.Message}");
            }
        }

        private static string FormatBoolean(bool? value)
            => value.HasValue ? (value.Value ? "Yes" : "No") : "Unknown";

    private static string PadCell(string? value, int width)
        => (value ?? "-").PadRight(width);

    private static string FormatTarget(Setting? setting)
    {
        if (setting?.Power == PowerStates.Off)
            return "Off";

        if (setting?.Temperature?.Celsius.HasValue == true)
            return $"{setting.Temperature.Celsius:0.0}C";

        if (!string.IsNullOrWhiteSpace(setting?.Mode))
            return setting.Mode!;

        return "-";
    }

    private static string FormatSettingSummary(TimetableBlock block)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(block.Setting?.Mode))
            parts.Add($"Mode={block.Setting.Mode}");

        if (block.Setting?.IsBoost.HasValue == true)
            parts.Add($"Boost={(block.Setting.IsBoost.Value ? "On" : "Off")}");

        if (block.GeolocationOverride.HasValue)
            parts.Add($"Geo={(block.GeolocationOverride.Value ? "Override" : "Follow")}");

        return parts.Count == 0 ? "-" : string.Join(", ", parts);
    }

    private static string NormalizeDayType(string? dayType)
        => string.IsNullOrWhiteSpace(dayType) ? "UNKNOWN" : dayType.Trim().ToUpperInvariant();

    private static int GetDaySortOrder(string dayType, string? timetableType)
    {
        var normalizedType = (timetableType ?? string.Empty).Trim().ToUpperInvariant();

        if (normalizedType == "ONE_DAY")
        {
            return dayType switch
            {
                "MONDAY_TO_SUNDAY" => 0,
                "DAILY" => 0,
                _ => 1
            };
        }

        if (normalizedType == "THREE_DAY")
        {
            return dayType switch
            {
                "MONDAY_TO_FRIDAY" => 0,
                "SATURDAY" => 1,
                "SUNDAY" => 2,
                _ => 3
            };
        }

        return dayType switch
        {
            "MONDAY" => 0,
            "TUESDAY" => 1,
            "WEDNESDAY" => 2,
            "THURSDAY" => 3,
            "FRIDAY" => 4,
            "SATURDAY" => 5,
            "SUNDAY" => 6,
            _ => 7
        };
    }

    private static string ToDisplayDayLabel(string dayType, string? timetableType)
    {
        return dayType switch
        {
            "MONDAY_TO_SUNDAY" => "Mon-Sun",
            "DAILY" => "Mon-Sun",
            "MONDAY_TO_FRIDAY" => "Mon-Fri",
            "MONDAY" => "Mon",
            "TUESDAY" => "Tue",
            "WEDNESDAY" => "Wed",
            "THURSDAY" => "Thu",
            "FRIDAY" => "Fri",
            "SATURDAY" => "Sat",
            "SUNDAY" => "Sun",
            _ when string.Equals(timetableType, "ONE_DAY", StringComparison.OrdinalIgnoreCase) => "Mon-Sun",
            _ => dayType
        };
    }

    private static string GetDefaultScheduleDays(string? timetableType)
    {
        return (timetableType ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            "ONE_DAY" => "Mon-Sun",
            "THREE_DAY" => "Mon-Fri, Sat, Sun",
            "SEVEN_DAY" => "Mon, Tue, Wed, Thu, Fri, Sat, Sun",
            _ => "unavailable"
        };
    }
}