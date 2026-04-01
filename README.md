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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;
using TadoNetApi.Application.Services;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure Tado credentials
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

        // Set up DI container
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

        try
        {
            // Authenticate with Tado
            Console.WriteLine("🔑 Requesting device authorisation...");
            var deviceCodeInfo = await authService.StartDeviceAuthorisationAsync(cancellationToken);
            await authService.WaitForAuthorisationAsync(deviceCodeInfo, maxWaitSeconds: 300, cancellationToken: cancellationToken);
            Console.WriteLine("✅ Device authorised successfully!");

            // Fetch user and home
            var user = await userService.GetMeAsync(cancellationToken);
            var homeId = (int)user.Homes.First().Id!;
            var home = await homeService.GetHomeAsync(homeId, cancellationToken);
            Console.WriteLine($"👤 User: {user.Name} ({user.Email})");
            Console.WriteLine($"🏠 Home: {home.Name} (ID: {home.Id})");

            // Fetch and display zones
            var zones = await zoneService.GetZonesAsync(homeId, cancellationToken);
            Console.WriteLine($"📊 {zones.Count} Zones found:");
            foreach (var zone in zones)
            {
                var state = await zoneService.GetZoneStateAsync(homeId, (int)zone.Id!, cancellationToken);
                var summary = await zoneService.GetZoneSummaryAsync(homeId, (int)zone.Id!, cancellationToken);
                
                Console.WriteLine($"   - Zone: {zone.Name} (ID: {zone.Id}, Type: {zone.Type})");
                Console.WriteLine($"       🌡 Current: {state.SensorDataPoints?.InsideTemperature?.Celsius}°C");
                Console.WriteLine($"       💧 Humidity: {state.SensorDataPoints?.Humidity?.Percentage}%");
                Console.WriteLine($"       🔥 Heating Power: {state.Setting?.Power}");
                
                if (summary?.Setting?.Temperature?.Celsius.HasValue == true)
                    Console.WriteLine($"       🎯 Target Temp: {summary.Setting.Temperature.Celsius}°C");
            }

            // Fetch and display devices
            var devices = await deviceService.GetDevicesAsync(homeId, cancellationToken);
            Console.WriteLine($"📦 Devices ({devices.Count}) in Home '{home.Name}':");
            foreach (var device in devices)
            {
                Console.WriteLine($"   • Device Type: {device.DeviceType}");
                Console.WriteLine($"       Connection: {(device.ConnectionState?.Value == true ? "Connected" : "Disconnected")}");
                Console.WriteLine($"       Battery: {device.BatteryState ?? "Unknown"}");
            }

            Console.WriteLine("🎉 Playground complete!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
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