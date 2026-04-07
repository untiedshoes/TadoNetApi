# Tado API

[![.NET](https://img.shields.io/badge/.NET-10.0-blue?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Build Status](https://github.com/untiedshoes/TadoNetApi/actions/workflows/dotnet.yml/badge.svg)](https://github.com/untiedshoes/TadoNetApi/actions/workflows/dotnet.yml)
[![Tests](https://img.shields.io/github/actions/workflow/status/untiedshoes/TadoNetApi/dotnet.yml?branch=main&label=unit%20tests&logo=xunit)](https://github.com/untiedshoes/TadoNetApi/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/github/license/untiedshoes/TadoNetApi.svg)](LICENSE)

> `TadoNetApi` is a .NET 10 library for working with the Tado smart heating platform through a clean, testable, service-oriented API.

It wraps the Tado API behind application and domain services, handles OAuth2 device authorization, maps DTOs into domain models, and keeps the integration code isolated from the rest of the application. The project is structured as a reusable SDK, but it also works well as a portfolio piece because it shows practical API integration work rather than just framework scaffolding.

If you want a machine-readable map of the SDK surface, including method signatures and the top-level model returned by each method, see `docs/sdk-surface.yaml`. It is intended as a companion to the README for contributors, integrators, and AI tooling.

Release history is tracked in `CHANGELOG.md`, with git tags intended to mirror released versions.

## What this project demonstrates

- Designing a maintainable .NET SDK over a third-party REST API
- Implementing OAuth2 device authorization and token lifecycle handling
- Separating domain, application, and infrastructure concerns cleanly
- Building resilient HTTP integrations with retries, cancellation, and error handling
- Using DTO-to-domain mapping instead of leaking transport models across the codebase
- Verifying behavior with unit tests, integration tests, and spec-parity reviews

---

## Features

- Clean separation between domain models, application services, and infrastructure concerns
- Typed support for key Tado areas including homes, zones, devices, weather, and user context
- Consumer-facing application services for common read and command workflows
- OAuth2 device authorization and token lifecycle handling via `TadoAuthService`
- Retry-aware and cancellation-aware HTTP integration with throttling support
- DTO-to-domain mapping so transport models do not leak through the public surface
- A runnable playground that uses the real auth flow and live API calls
- Unit and integration tests covering mapping, service behavior, and command paths

---

## Why this project?
Tado exposes a useful API surface, but there is no official .NET SDK that presents it in a way that feels natural to use in a modern application.

This project exists to close that gap. It turns a relatively awkward external API into a set of clear service contracts and domain models that are easier to test, extend, and reason about. It also gave me a real integration problem to solve end to end: authentication, transport concerns, mapping, command payloads, edge cases, and incremental endpoint coverage.

---

## Quick C# Example

The library is designed to be consumed through dependency injection and application services. A minimal authenticated flow looks like this:

```csharp
var services = new ServiceCollection();

services.AddTadoInfrastructure(new TadoApiConfig
{
    Username = Environment.GetEnvironmentVariable("TADO_USERNAME") ?? string.Empty,
    Password = Environment.GetEnvironmentVariable("TADO_PASSWORD") ?? string.Empty
});

using var provider = services.BuildServiceProvider();

var authService = provider.GetRequiredService<ITadoAuthService>();
var userService = provider.GetRequiredService<UserAppService>();

var deviceCode = await authService.StartDeviceAuthorisationAsync();

await authService.WaitForDeviceTokenAsync(
    deviceCode.DeviceCode,
    deviceCode.Interval,
    deviceCode.ExpiresIn);

var me = await userService.GetMeAsync();

foreach (var home in me.Homes ?? [])
{
    Console.WriteLine(home.Name);
}
```

---

## Playground
A runnable console app is included at:

```
TadoNetApi.Playground
```

The playground is intentionally simple. It is there to exercise the real authentication flow, query live Tado data, and show the intended usage pattern for the library without hiding everything behind sample-only helper code.

---

## Tech Stack

- .NET 10 (`net10.0`)
- C# with nullable reference types and implicit usings enabled
- Clean Architecture across Domain, Application, Infrastructure, and Playground layers
- `Microsoft.Extensions.Http` for DI-friendly HTTP client composition
- OAuth2 device authorization against the Tado platform
- `Newtonsoft.Json` for API serialization, DTO binding, and custom enum converters
- `Microsoft.Extensions.Logging.Console` for playground/runtime logging
- Async/await with cancellation-token support across service boundaries
- xUnit + Moq + Coverlet for unit testing and coverage collection
- Community Tado API v2 OpenAPI/Swagger definitions used for endpoint parity review

---

## Architecture Overview

The codebase is split into four layers:

- Domain → Core business models and rules
- Application → Use cases and orchestration
- Infrastructure → External API integration
- Presentation (Playground) → Example usage

That split keeps the transport details and API quirks out of the rest of the codebase, while making the integration testable and easier to evolve as more endpoints are added.

In practice, this gives:
- Separation of concerns
- Testability
- Long-term maintainability

```text
TadoNetApi/
├─ src/
│  ├─ TadoNetApi.Domain/         # Core business entities, enums, and interfaces
│  │  ├─ Entities/               # Home, Zone, Device, Weather, User, State, ZoneSummary, etc.
│  │  ├─ Enums/                  # DeviceTypes, PowerStates, TerminationTypes, etc.
│  │  └─ Interfaces/             # Domain service contracts (IHomeService, IZoneService, etc.)
│  ├─ TadoNetApi.Application/    # Consumer-facing application services
│  │  └─ Services/               # HomeAppService, ZoneAppService, DeviceAppService, etc.
│  └─ TadoNetApi.Infrastructure/ # External API integration and transport concerns
│     ├─ Auth/                   # TadoAuthService for OAuth2 device authorization
│     ├─ Config/                 # TadoApiConfig for credentials and settings
│     ├─ Converters/             # JSON converters for enums
│     ├─ Dtos/                   # API request/response DTOs
│     ├─ Extensions/             # DI registration extensions
│     ├─ Http/                   # TadoHttpClient with auth, retries, throttling
│     ├─ Mappers/                # DTO to domain entity mapping
│     └─ Services/               # Concrete service implementations (TadoZoneService, etc.)
├─ TadoNetApi.Playground/        # Console app demonstrating API usage
├─ tests/                        # Unit and integration tests
│  ├─ Application/Services/      # AppService delegation/orchestration tests
│  ├─ Domain/                    # Entity tests
│  ├─ Infrastructure/Services/   # HTTP, mapping, and command behavior tests
│  ├─ Integration/               # Real API integration tests
│  └─ Mocks/                     # Mock services and HTTP helpers
├─ docs/                         # Machine-readable SDK reference and supporting docs
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

## Service Reference

Application services are the main entry point for consumers. In practice, these are the types a host application, or the playground, resolves from DI.

If you want a machine-readable map of the SDK surface, including method signatures and the top-level model returned by each method, see `docs/sdk-surface.yaml`. It is intended as a companion to the README for contributors, integrators, and AI tooling.

### Application Services

| Service | Responsibility | Main Methods |
|-------|-------|-------|
| UserAppService | Retrieve the current authenticated user and home context | `GetMeAsync` |
| HomeAppService | Read home data, home users, comfort, heating, flow-temperature optimisation, and diagnostic indicators, and manage presence state | `GetHomeAsync`, `GetHomeStateAsync`, `GetUsersAsync`, `GetAirComfortAsync`, `GetIncidentDetectionAsync`, `GetHeatingCircuitsAsync`, `GetHeatingSystemAsync`, `GetFlowTemperatureOptimisationAsync`, `SetHomePresenceAsync`, `ResetHomePresenceAsync`, `SetAwayRadiusInMetersAsync`, `SetIncidentDetectionAsync`, `SetHomeDetailsAsync`, `SetFlowTemperatureOptimisationAsync` |
| ZoneAppService | Read zone data, away-configuration settings, day reports, and send zone-level and home-level overlay commands | `GetZonesAsync`, `GetZoneAsync`, `GetZoneStateAsync`, `GetZoneSummaryAsync`, `GetZoneCapabilitiesAsync`, `GetZoneControlAsync`, `GetDefaultZoneOverlayAsync`, `GetEarlyStartAsync`, `GetAwayConfigurationAsync`, `GetZoneDayReportAsync`, `GetZoneTemperatureOffsetAsync`, `CreateZoneAsync`, `SetZoneOverlaysAsync`, `DeleteZoneOverlaysAsync`, `DeleteZoneOverlayAsync`, `SetHeatingCircuitAsync`, `SetEarlyStartAsync`, `SetOpenWindowDetectionAsync`, `ActivateOpenWindowAsync`, `ResetOpenWindowAsync`, `SetZoneDetailsAsync`, `SetDefaultZoneOverlayAsync`, `SetAwayConfigurationAsync`, `SetHeatingTemperatureCelsiusAsync`, `SetHeatingTemperatureCelsiusAsync(DurationModes, timer)`, `SetHeatingTemperatureFahrenheitAsync`, `SetHotWaterTemperatureCelsiusAsync`, `SetHotWaterTemperatureFahrenheitAsync`, `SwitchHeatingOffAsync`, `SwitchHotWaterOffAsync` |
| DeviceAppService | Read device and mobile-device data and send device-level commands | `GetDevicesAsync`, `GetDeviceListAsync`, `GetDeviceAsync`, `GetZoneTemperatureOffsetAsync`, `GetMobileDevicesAsync`, `GetMobileDeviceAsync`, `GetMobileDeviceSettingsAsync`, `GetZoneMeasuringDeviceAsync`, `MoveDeviceToZoneAsync`, `SetZoneMeasuringDeviceAsync`, `DeleteMobileDeviceAsync`, `SetMobileDeviceSettingsAsync`, `SetDeviceChildLockAsync`, `SayHiAsync`, `SetZoneTemperatureOffsetCelsiusAsync`, `SetZoneTemperatureOffsetFahrenheitAsync` |
| WeatherAppService | Read weather data for a home | `GetWeatherAsync` |

### Infrastructure Services

Infrastructure services are the API-facing implementations behind the domain interfaces. They handle HTTP calls, auth, DTO serialization, mapping, and command payloads.

| Service | Responsibility | Main Methods | Interface |
|-------|-------|-------|-------|
| TadoUserService | User endpoint integration | `GetMeAsync` | `IUserService` |
| TadoHomeService | Home retrieval, home users, air comfort, heating, flow-temperature optimisation, incident detection, home state, presence updates, away-radius changes, incident-detection updates, home-details updates, and flow-temperature updates | `GetHomeAsync`, `GetHomeStateAsync`, `GetUsersAsync`, `GetAirComfortAsync`, `GetIncidentDetectionAsync`, `GetHeatingCircuitsAsync`, `GetHeatingSystemAsync`, `GetFlowTemperatureOptimisationAsync`, `SetHomePresenceAsync`, `ResetHomePresenceAsync`, `SetAwayRadiusInMetersAsync`, `SetIncidentDetectionAsync`, `SetHomeDetailsAsync`, `SetFlowTemperatureOptimisationAsync` | `IHomeService` |
| TadoZoneService | Zone retrieval, away-configuration reads and writes, day-report reads, plus zone creation, single-zone and bulk overlay commands, heating-circuit updates, early-start, open-window-detection commands, zone-details/default-overlay updates, and overlay-temperature commands | `GetZonesAsync`, `GetZoneAsync`, `GetZoneStateAsync`, `GetZoneSummaryAsync`, `GetZoneCapabilitiesAsync`, `GetZoneControlAsync`, `GetDefaultZoneOverlayAsync`, `GetEarlyStartAsync`, `GetAwayConfigurationAsync`, `GetZoneDayReportAsync`, `GetZoneTemperatureOffsetAsync`, `CreateZoneAsync`, `SetZoneOverlaysAsync`, `DeleteZoneOverlaysAsync`, `DeleteZoneOverlayAsync`, `SetHeatingCircuitAsync`, `SetEarlyStartAsync`, `SetOpenWindowDetectionAsync`, `ActivateOpenWindowAsync`, `ResetOpenWindowAsync`, `SetZoneDetailsAsync`, `SetDefaultZoneOverlayAsync`, `SetAwayConfigurationAsync`, `SetHeatingTemperatureCelsiusAsync`, `SetHeatingTemperatureCelsiusAsync(DurationModes, timer)`, `SetHeatingTemperatureFahrenheitAsync`, `SetHotWaterTemperatureCelsiusAsync`, `SetHotWaterTemperatureFahrenheitAsync`, `SwitchHeatingOffAsync`, `SwitchHotWaterOffAsync` | `IZoneService` |
| TadoDeviceService | Device and mobile-device retrieval plus zone move, measuring-device updates, mobile-device deletion, settings updates, child-lock, identify, and offset commands | `GetDevicesAsync`, `GetDeviceListAsync`, `GetDeviceAsync`, `GetZoneTemperatureOffsetAsync`, `GetMobileDevicesAsync`, `GetMobileDeviceAsync`, `GetMobileDeviceSettingsAsync`, `GetZoneMeasuringDeviceAsync`, `MoveDeviceToZoneAsync`, `SetZoneMeasuringDeviceAsync`, `DeleteMobileDeviceAsync`, `SetMobileDeviceSettingsAsync`, `SetDeviceChildLockAsync`, `SayHiAsync`, `SetZoneTemperatureOffsetCelsiusAsync`, `SetZoneTemperatureOffsetFahrenheitAsync` | `IDeviceService` |
| TadoWeatherService | Weather endpoint integration | `GetWeatherAsync` | `IWeatherService` |
| TadoAuthService | OAuth2 device authorization and token lifecycle | `StartDeviceAuthorisationAsync`, `WaitForDeviceTokenAsync`, `GetAccessTokenAsync` | `ITadoAuthService` |

Service flow in this project: Playground -> Application Services -> Domain Interfaces -> Infrastructure Services -> Tado API.

---

## Getting Started

The quickest way to explore the library is to run the playground against a real Tado account.

### 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- A Tado account with valid credentials

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

If you want verbose `HttpClient` logging in the playground, you can also set:

```bash
export TADO_VERBOSE_HTTP_LOGS="true"
```

### 4. Run the Playground

```bash
cd TadoNetApi.Playground
dotnet run
```

The playground will:

- Perform OAuth2 device authorization with Tado
- Fetch the authenticated user and associated homes
- Query live zone, device, and weather data
- Show the intended application-service usage pattern
- Exercise edge cases already handled by the library, such as missing overlays

### Playground Example

The example below is a trimmed version of the runnable console flow. It shows the intended DI setup and a typical first query path.

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

For the complete runnable example, see [TadoNetApi.Playground/Program.cs](TadoNetApi.Playground/Program.cs).

---

## Testing

The test project covers domain behavior, mapping, service-level logic, and selected integration paths against the real API surface.

- Unit tests live in the `tests` project
- xUnit is used as the test framework
- Moq is used for isolation around infrastructure dependencies
- Coverlet is included for coverage collection

Run tests with:
```bash
dotnet test
```

---

## Throttling Behaviour

The SDK automatically retries `429 Too Many Requests` responses using `Retry-After` when the API provides it, and otherwise falls back to the configured backoff settings.

If the retry budget is exhausted, the SDK throws `RequestThrottledException` so callers can distinguish throttling from other API failures. That exception exposes the parsed throttling metadata when the API sends it, including:

- request URI
- retry-after delay or retry-after date
- rate-limit policy name
- quota and window size
- remaining requests
- reset time

This is in place so SDK consumers can log, surface, or react to throttling with more context than a generic HTTP failure.

---

## Notes

- API calls include automatic retry logic and rate limiting awareness
- Supports OAuth2 device authorization flow for secure authentication
- Handles API edge cases like missing zone overlays (returns null instead of errors)
- Fully compatible with CancellationToken for safe async operations
- Designed for extensibility to add additional Tado API endpoints
- Playground demonstrates real-world usage with proper error handling
- All services use dependency injection for testability and flexibility

## Roadmap / Spec Gap Analysis

The list below reflects the current gap between this library and the community managed tado API v2 OpenAPI definition.

##### Alignment / Review

##### Home / User / Invitation Services

- [ ] Implement invitation service methods -> `GET /homes/{homeId}/invitations`, `POST /homes/{homeId}/invitations`, `DELETE /homes/{homeId}/invitations/{invitationToken}`, `POST /homes/{homeId}/invitations/{invitationToken}/resend`

##### Zone Services

- [ ] Implement schedule/timetable operations -> `GET|PUT /homes/{homeId}/zones/{zoneId}/schedule/activeTimetable`, `GET /homes/{homeId}/zones/{zoneId}/schedule/timetables`, `GET /homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}`, `GET|PUT /homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}/blocks/{dayType}`, `GET /homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}/blocks`

##### Device / Mobile Device Services

- [ ] Implement installation endpoints -> `GET /homes/{homeId}/installations`, `GET /homes/{homeId}/installations/{installationId}`

##### Specialized / Optional Services

- [ ] Considering adding bridge service -> `GET /bridges/{bridgeId}`
- [ ] Considering adding boiler-by-bridge service -> `GET /homeByBridge/{bridgeId}/boilerInfo`, `GET|PUT /homeByBridge/{bridgeId}/boilerMaxOutputTemperature`, `GET /homeByBridge/{bridgeId}/boilerWiringInstallationState`

##### Testing / Documentation

- [ ] Add unit tests for remaining command payload variants (Fahrenheit, hot water, off/wrapper flows)

---

## Important Warning

Tado has not officially released a public API for general third-party developer use.

This library is built around a community-documented API surface derived from observing traffic used by Tado's own web application and related clients. That means Tado can change or remove endpoints, payloads, authentication behavior, or feature support at any time without notice.

Before using any Tado API in your own software, make sure you understand the operational risk: this library, the underlying unofficial API behavior, and your own integration can break without advance warning.

## Tado X Support

This library currently focuses on the community-documented `https://my.tado.com/api/v2` surface and does not aim to support Tado X homes end-to-end today.

As documented by kritsel, Tado X homes still use parts of `my.tado.com/api/v2` for some features, but the core room and device management flows are handled through a different API surface hosted at `https://hops.tado.com`, which is outside the scope of the current community spec.

That limitation is mainly practical: without access to a Tado X-enabled home, it is difficult me to verify behavior, maintain mappings confidently, or keep support accurate as the platform evolves.

If you have a Tado X-enabled home and want to help move this repo forward, contributions are welcome. In particular, verified endpoint behavior, payload samples, tests, and documentation updates would all be useful.

For background on the current limitation, see the upstream note here: [tado X support rationale](https://github.com/kritsel/tado-openapispec-v2/wiki/tado-X).

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