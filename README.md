# TadoNetApi

[![.NET](https://img.shields.io/badge/.NET-10-blue)](https://dotnet.microsoft.com/)  
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)  

`TadoNetApi` is a Clean Architecture .NET 10 library that provides a strongly-typed wrapper for the **Tado V2 API**.  
It follows **SOLID principles**, with full separation of concerns, layered architecture, testable design, and built-in throttling handling.

---

## Features

- Authenticate with Tado using config-based credentials  
- Fetch **user, homes, zones, devices, and weather**  
- Read and set **zone overlays / target temperatures**  
- Respect **Tado API throttling** with retries & exponential backoff  
- Fully **cancellation-token aware** for safe async operations  
- Clean Architecture + SOLID + testable design  
- Designed for **extensibility**: easily add schedules, programs, or additional endpoints  

---

## Architecture Overview

```text
TadoNetApi/
├─ Domain/          # Core entities and interfaces
│  ├─ Entities/     # Home, Zone, Device, Weather, Overlay
│  └─ Interfaces/   # IHomeService, IZoneService, IUserService, etc.
│
├─ Infrastructure/  # Implementation of services and API clients
│  ├─ Config/       # TadoApiConfig for credentials and retry settings
│  ├─ Dtos/         # API request/response DTOs
│  ├─ Http/         # TadoHttpClient with authentication & throttling
│  ├─ Mappers/      # DTO → Domain mapping
│  └─ Services/     # Concrete implementations of Domain interfaces
│
├─ Playground/      # Example console app demonstrating API usage
├─ tests/           # xUnit + Moq unit tests
└─ .gitignore
```

---

## Layer Responsibilities

| Layer | Responsibility |
|-------|----------------|
| Domain | Core domain entities (`Home`, `Zone`, `Device`, `Weather`, `Overlay`) and value objects (e.g., `Temperature`); encapsulates business rules; has **no external dependencies**. |
| Application | Interfaces (contracts) such as `IHomeService`, `IZoneService`, `IDeviceService`; orchestrates business logic; maps Infrastructure DTOs to Domain models; use cases like `FetchHomesUseCase`. |
| Infrastructure | Concrete implementations of external concerns: `TadoHttpClient` (HTTP), `TadoAuthProvider` (auth), optional caching services; maps API responses to DTOs and Domain models; handles throttling, retries, and overlays. |
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