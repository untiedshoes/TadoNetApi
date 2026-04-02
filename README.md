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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;

var config = new TadoApiConfig
{
    Username = Environment.GetEnvironmentVariable("TADO_USERNAME") ?? "",
    Password = Environment.GetEnvironmentVariable("TADO_PASSWORD") ?? "",
    MaxRetries = 5,
    InitialRetryDelayMs = 1000
};

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddTadoInfrastructure(config);
var provider = services.BuildServiceProvider();

var authService = provider.GetRequiredService<ITadoAuthService>();
var userService = provider.GetRequiredService<UserAppService>();
var zoneService = provider.GetRequiredService<ZoneAppService>();
var weatherService = provider.GetRequiredService<IWeatherService>();

// 1) Device authorization flow
var deviceCode = await authService.StartDeviceAuthorisationAsync();
Console.WriteLine($"Visit: {deviceCode.VerificationUriComplete ?? deviceCode.VerificationUri}");
Console.WriteLine($"Code: {deviceCode.UserCode}");

await authService.WaitForDeviceTokenAsync(
    deviceCode.DeviceCode,
    deviceCode.Interval,
    deviceCode.ExpiresIn);

// 2) Fetch user + first home
var me = await userService.GetMeAsync();
var homeId = (int)(me.Homes?.FirstOrDefault()?.Id ?? throw new InvalidOperationException("No homes found"));

// 3) Query zone + weather data
var zones = await zoneService.GetZonesAsync(homeId);
var weather = await weatherService.GetWeatherAsync(homeId);

Console.WriteLine($"User: {me.Name}");
Console.WriteLine($"Zones: {zones.Count}");
Console.WriteLine($"Weather: {weather.WeatherState?.Value ?? weather.WeatherState?.CurrentType}");

```

For the complete runnable example, see [src/TadoNetApi.Playground/Program.cs](src/TadoNetApi.Playground/Program.cs).

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

#### TODO: Send Command Parity

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