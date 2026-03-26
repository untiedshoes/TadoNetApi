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
│  ├─ Domain/           # Tests for domain entities
│  ├─ Services/         # Tests for service classes (HomeService, ZoneService, etc.)
│  ├─ Mocks/            # Mock implementations for ITadoHttpClient & ITadoAuthService.
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

- Authenticate with Tado
- Fetch the user, homes, zones, devices, weather
- Display current overlays
- Set example target temperatures

### Usage (Playground Example)

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Extensions;
using TadoNetApi.Infrastructure.Http;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Starting Tado Playground...");

        // Read from environment
        var config = new TadoApiConfig
        {
            Username = Environment.GetEnvironmentVariable("TADO_USERNAME") ?? "",
            Password = Environment.GetEnvironmentVariable("TADO_PASSWORD") ?? ""
        };

        // DI container
        var services = new ServiceCollection()
            .AddTadoInfrastructure(config)
            .BuildServiceProvider();

        // Resolve HttpClient
        var tadoClient = services.GetRequiredService<ITadoHttpClient>();

        try
        {
            var user = await tadoClient.GetAsync("me");
            Console.WriteLine("Fetching user...");
            Console.WriteLine(user);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred:");
            Console.WriteLine(ex.Message);
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

- API calls are rate-limit aware with automatic retries
- Currently supports OAuth2 device authorization flow.
- Ensure your Tado account is active and credentials are correct.
- Playground demonstrates real-world usage of all services
- Fully compatible with CancellationToken for safe async calls
- Designed for extensibility to add additional Tado endpoints

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