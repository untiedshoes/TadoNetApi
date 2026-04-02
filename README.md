# TadoNetApi

[![.NET](https://img.shields.io/badge/.NET-10.0-blue?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Build Status](https://github.com/untiedshoes/TadoNetApi/actions/workflows/dotnet.yml/badge.svg)](https://github.com/untiedshoes/TadoNetApi/actions/workflows/dotnet.yml)
[![Tests](https://img.shields.io/github/actions/workflow/status/untiedshoes/TadoNetApi/dotnet.yml?branch=main&label=unit%20tests&logo=xunit)](https://github.com/untiedshoes/TadoNetApi/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/untiedshoes/TadoNetApi.svg)](LICENSE)
[![Code Quality](https://img.shields.io/badge/code%20quality-A-brightgreen.svg)](#) <!-- Optional, placeholder for future SonarCloud/CodeFactor integration -->

> `TadoNetApi` is a .NET 10 library providing a Clean Architecture implementation for the Tado Smart Heating system. It allows full interaction with Tado homes, zones, devices, schedules, and weather, including OAuth2 device authorization, overlays, and API throttling.  

The library is designed with **SOLID principles** and **reliability in mind**, featuring retry-aware HTTP clients, cancellation token support, and comprehensive unit and integration tests.

---

## Features

- **Core Domain Entities**: Home, Zone, Device, Weather, User, State, ZoneSummary, and related value objects
- **Application Services**: HomeAppService, ZoneAppService, DeviceAppService, UserAppService for business logic orchestration
- **Infrastructure Services**: TadoZoneService, TadoDeviceService, etc. with full API integration
- **Authentication**: OAuth2 device authorization flow with TadoAuthService
- **HTTP Client**: TadoHttpClient with automatic bearer token management, retry logic, throttling, and cancellation support
- **Mapping Layer**: Comprehensive DTO-to-domain mappers for all entities
- **Clean Architecture**: Strict separation between Domain, Application, and Infrastructure layers
- **Playground Console App**: Complete demonstration of API usage with real authentication and data fetching
- **Error Handling**: Graceful handling of API edge cases (e.g., missing overlays, null responses)
- **Testing**: xUnit + Moq for unit tests, integration tests for real API validation

---

## TODO: Send Command Parity

- [ ] Implement `SetOpenWindowAsync` -> `POST homes/{homeId}/zones/{zoneId}/state/openWindow/activate`
///   Activates temporary open-window mode on a zone and lets the API switch behavior accordingly.
- [ ] Implement `ResetOpenWindowAsync` -> `DELETE homes/{homeId}/zones/{zoneId}/state/openWindow`
///   Clears open-window override and returns zone control to normal logic.
- [ ] Review/align home presence command parity (`SetHomePresence` in reference uses `PUT homes/{homeId}/presenceLock`)
///   Confirm endpoint contract differences and align request path/method with intended presence-lock semantics.
- [ ] Implement `SetEarlyStartAsync` -> `PUT homes/{homeId}/zones/{zoneId}/earlyStart`
///   Enables or disables early-start preheating for a zone.
- [ ] Implement `SayHiAsync` (device identify) -> `POST devices/{deviceId}/identify`
///   Triggers physical identify action on a specific device for pairing and troubleshooting.
- [ ] Implement `SetDeviceChildLockAsync` -> `PUT devices/{deviceId}/childLock`
///   Sends `childLockEnabled` state to lock or unlock device controls.
- [ ] Implement `SetZoneTemperatureOffsetCelsiusAsync` -> `PUT devices/{deviceId}/temperatureOffset`
///   Writes Celsius calibration offset for the selected device sensor.
- [ ] Implement `SetZoneTemperatureOffsetFahrenheitAsync` -> `PUT devices/{deviceId}/temperatureOffset`
///   Writes Fahrenheit calibration offset for the selected device sensor.
- [ ] Implement `SetHeatingTemperatureCelsiusAsync` (default duration wrapper)
///   Convenience API that sets heating target in Celsius until next manual change.
- [ ] Implement `SetHeatingTemperatureCelsiusAsync` (duration/timer overload)
///   Sets heating target in Celsius with explicit termination mode and optional timer seconds.
- [ ] Implement `SetHeatingTemperatureFahrenheitAsync` (default duration wrapper)
///   Convenience API that sets heating target in Fahrenheit until next manual change.
- [ ] Implement `SetHeatingTemperatureFahrenheitAsync` (duration/timer overload)
///   Sets heating target in Fahrenheit with explicit termination mode and optional timer seconds.
- [ ] Implement `SetHotWaterTemperatureCelsiusAsync` (duration/timer)
///   Sets hot-water target in Celsius using overlay termination settings.
- [ ] Implement `SetHotWaterTemperatureFahrenheitAsync` (duration/timer)
///   Sets hot-water target in Fahrenheit using overlay termination settings.
- [ ] Implement shared `SetTemperatureAsync` overlay command core -> `PUT homes/{homeId}/zones/{zoneId}/overlay`
///   Centralizes overlay payload creation (`setting`, `power`, `temperature`, `termination`) to avoid duplicated command logic.
- [ ] Implement `SwitchHeatingOffAsync` (default duration wrapper)
///   Convenience API to turn heating off until next manual change.
- [ ] Implement `SwitchHeatingOffAsync` (duration/timer overload)
///   Turns heating off for explicit duration mode and optional timer window.
- [ ] Implement `SwitchHotWaterOffAsync` (duration/timer)
///   Turns hot water off using the same overlay and termination model.
- [ ] Add DTOs/builders for overlay command payload (`setting`, `termination`, timer duration)
///   Introduces typed request models and helper builders to guarantee API-compatible JSON.
- [ ] Add unit tests for all new send commands (payload shape, endpoint, HTTP verb)
///   Verifies endpoint path, method, expected status handling, and serialized request body.
- [ ] Add integration tests for at least one heating set command and one device set command
///   Validates end-to-end behavior against live API for a zone overlay and a device-level command.

---

## Architecture Overview

```text
TadoNetApi/
├─ Domain/              # Core business entities and enums
│  ├─ Entities/         # Home, Zone, Device, Weather, User, State, ZoneSummary, etc.
│  └─ Enums/            # DeviceTypes, PowerStates, TerminationTypes, etc.
│
├─ Application/         # Application services and interfaces
│  ├─ Services/         # AppServices (HomeAppService, ZoneAppService, DeviceAppService, UserAppService)
│  └─ Interfaces/       # IService interfaces (IZoneService, IDeviceService, etc.)
│
├─ Infrastructure/      # External concerns and implementations
│  ├─ Auth/             # TadoAuthService for OAuth2 device authorization
│  ├─ Config/           # TadoApiConfig for credentials and settings
│  ├─ Converters/       # JSON converters for enums
│  ├─ Dtos/             # API request/response DTOs
│  ├─ Extensions/       # DI registration extensions
│  ├─ Http/             # TadoHttpClient with auth, retries, throttling
│  ├─ Mappers/          # DTO to Domain entity mapping
│  └─ Services/         # Concrete service implementations (TadoZoneService, etc.)
│
├─ Playground/          # Console app demonstrating API usage
├─ Tests/               # Unit and integration tests
│  ├─ Domain/           # Entity tests
│  ├─ Services/         # Service tests
│  ├─ Mocks/            # Mock services for testing
│  └─ Integration/      # Real API integration tests
└─ .gitignore
```

---

## Layer Responsibilities

| Layer | Responsibility |
|-------|----------------|
| Domain | Core business entities (Home, Zone, Device, Weather, User, State, ZoneSummary) and value objects (e.g., Temperature, Address); business enums (DeviceTypes, PowerStates); encapsulates business rules; has no external dependencies. |
| Application | Application services (AppServices) that orchestrate business logic and use cases; defines service interfaces (IZoneService, IDeviceService); acts as a bridge between Domain and Infrastructure; handles domain-specific operations. |
| Infrastructure | Concrete implementations of external concerns: HTTP clients (TadoHttpClient), authentication (TadoAuthService), API DTOs, mappers (DTO → Domain), JSON converters; handles retries, throttling, and API-specific logic; depends on external libraries. |
| Playground | Example console application demonstrating real API usage; integrates all layers via DI; showcases authentication, fetching homes/zones/devices, displaying current states and overlays; manual testing of the library. |
| Tests | Unit tests (xUnit + Moq) for isolated logic and integration tests for real API calls; validates entity mapping, service behavior, error handling, and end-to-end flows. |

---

## Getting Started

### 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)  
- Tado V3 account and credentials  

### 2. Clone the repo

```bash
git clone https://github.com/untiedshoes/TadoNetApi.git
cd TadoNetApi
```

### 3. Configure your Tado credentials
Set your Tado credentials as environment variables:

```bash
export TADO_USERNAME="your-email@example.com"
export TADO_PASSWORD="your-password"
```

### 4. Run the Playground

```bash
cd src/TadoNetApi.Playground
dotnet run
```

This will:

- Perform OAuth2 device authorization with Tado
- Fetch and display user information and associated homes
- List all zones with current temperature, humidity, heating power, and overlay details (if active)
- Display device information including connection status, battery state, and capabilities
- Demonstrate error handling for API edge cases (e.g., missing overlays)

### Usage (Playground Example)

```csharp
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
```

---

### Testing
- Unit tests are located in the tests folder
- Uses xUnit + Moq
- Tests cover domain logic, mappers, and service behavior

Run tests with:
```bash
dotnet test
```

---

### Notes

- API calls include automatic retry logic and rate limiting awareness
- Supports OAuth2 device authorization flow for secure authentication
- Handles API edge cases like missing zone overlays (returns null instead of errors)
- Fully compatible with CancellationToken for safe async operations
- Designed for extensibility to add additional Tado API endpoints
- Playground demonstrates real-world usage with proper error handling
- All services use dependency injection for testability and flexibility

---

## References

- [Tado API v2 Spec (community)](https://github.com/kritsel/tado-openapispec-v2)
- [.NET Clean Architecture guidelines](https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

## License

MIT

---

## Author

Craig Richards
Backend Developer | .NET Engineer