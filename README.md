# TadoNetApi

[![.NET](https://img.shields.io/badge/.NET-10-blue)](https://dotnet.microsoft.com/)  
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)  

> `TadoNetApi` is a .NET 10 library that provides a strongly-typed, Clean Architecture implementation for interacting with the Tado Smart Heating API
. It supports homes, zones, devices, schedules, weather, overlays, and full authentication using Tado’s OAuth2 device flow.

---

## Features

- Core Domain Entities: Home, Zone, Device, Weather, Schedule
- Services: HomeService, ZoneService, DeviceService, WeatherService, ScheduleService
- Authentication via TadoAuthService (Device Authorization + OAuth2 token management)
- HttpClient integration: automatically handles bearer token, throttling, retries, and cancellation tokens
- Clean Architecture and SOLID principles: clear separation between Domain, Application, Infrastructure, Playground, and Tests
- Full Playground console app to demonstrate API usage
- Supports reading and modifying heating schedules and overlays
- xUnit + Moq tests for unit and integration testing

---

## Architecture Overview

```text
TadoNetApi/
├─ Domain/          # Core entities and interfaces
│  ├─ Entities/     # Home, Zone, Device, Weather, Overlay, Schedule
│  └─ Interfaces/   # IHomeService, IZoneService, IDeviceService, IScheduleService, IUserService, etc.
│
├─ Infrastructure/  # Implementation of services and API clients
│  ├─ Config/       # TadoApiConfig for credentials, retries, and throttling
│  ├─ Dtos/         # API request/response DTOs (Home, Zone, Device, Weather, Overlay, Schedule)
│  ├─ Http/         # TadoHttpClient with authentication, throttling, retries
│  ├─ Mappers/      # DTO → Domain mapping
│  └─ Services/     # Concrete implementations of Domain interfaces (Home, Zone, Device, Schedule, Weather, Overlay)
│
├─ Playground/      # Example console app demonstrating API usage
├─ Tests/
│  ├─ Domain/           # Tests for domain entities (optional, mostly validation)
│  ├─ Services/         # Tests for service classes (HomeService, ZoneService, etc.)
│  ├─ Mocks/            # Mock implementations for ITadoHttpClient, etc.
│  ├─ TadoNetApi.Tests.csproj
└─ .gitignore
```

---

## Layer Responsibilities

| Layer | Responsibility |
|-------|----------------|
| Domain | Core domain entities (Home, Zone, Device, Weather, Overlay, Schedule) and value objects (e.g., Temperature); encapsulates business rules; has no **external dependencies**. |
| Application | Interfaces (contracts) such as `IHomeService`, `IZoneService`, `IDeviceService`; orchestrates business logic; maps Infrastructure DTOs to Domain models; use cases like `FetchHomesUseCase`. |
| Infrastructure | Concrete implementations of external concerns: `TadoHttpClient` (HTTP), `TadoAuthService` (authentication), optional caching services; maps API responses to DTOs and Domain models; handles throttling, retries, and overlays. |
| Extensions | Dependency injection wiring; provides a single entry point to register all services in DI containers; keeps Program.cs clean. |
| Playground | Example console application demonstrating usage of `TadoNetApi`; manual testing of API flows; integrates DI and prints Domain objects; showcases overlays and zone control. |
| Tests | **Unit tests** with xUnit and Moq (isolated Domain/Application logic) and **integration tests** (real API calls); validates entity mapping, service behavior, and use cases. |

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
Edit TadoApiConfig.cs:

```csharp
var tadoConfig = new TadoApiConfig
{
    Username = "your-email@example.com",
    Password = "your-password",
    MaxRetries = 5,
    InitialRetryDelayMs = 1000
};
```

### 4. Run the Playground

```bash
cd src/TadoNetApi.Playground
dotnet run
```
This will:

- Authenticate with Tado
- Fetch the user, homes, zones, devices, weather
- Display current overlays
- Set example target temperatures

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

- API calls are rate-limit aware with automatic retries
- Playground demonstrates real-world usage of all services
- Fully compatible with CancellationToken for safe async calls
- Designed for extensibility to add additional Tado endpoints


---

## References

- Original inspiration: [TadoApi by Koen Zomers](https://github.com/KoenZomers/TadoApi)
- [Tado API v2 Spec (community)](https://github.com/kritsel/tado-openapispec-v2)
- [.NET Clean Architecture guidelines](https://learn.microsoft.com/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

---

## License

MIT

---

## Author

Craig Richards
Backend Developer | .NET Engineer