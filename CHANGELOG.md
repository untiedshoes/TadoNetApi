# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project follows Semantic Versioning.

## [1.0.2] - 2026-04-22

### Added
- Integration tests for failure scenarios (e.g., invalid token) in `ZoneIntegrationTests`.
- Explicit coverage for negative API responses and error handling paths.

### Changed
- Improved robustness by verifying error handling in integration tests.

## [1.0.1] - 2026-04-21

### Fixed
- Added `HttpRequestException` catch blocks to `GetHomeAsync`, `GetHomeStateAsync`, `GetWeatherAsync`, `GetBridgeAsync`, `GetBoilerInfoAsync`, `GetBoilerMaxOutputTemperatureAsync`, and `GetBoilerWiringInstallationStateAsync`; all now surface as `TadoApiException(ServiceUnavailable)` for consistent error handling.

### Changed
- Retry log messages in `RetryDelegatingHandler` now include the HTTP method and request URI for easier diagnostics.

### Tests
- Added `FakeHttpMessageHandler` in `tests/Fakes/` for direct `HttpClient`-pipeline testing.
- Added `TadoHttpClientTests` covering 200 OK deserialization, 4xx/5xx responses, timeout, caller cancellation, network failure, malformed JSON, null deserialization, and throttle exhaustion.
- Extended failure-path coverage across `TadoZoneService`, `TadoHomeService`, `TadoDeviceService`, `TadoWeatherService`, `TadoBridgeService`, and `TadoBoilerByBridgeService`.
- Added XML `<summary>` documentation to all test methods across the test suite.

## [1.0.0] - 2026-04-08

### Added
- Added home installation read support for `GET /homes/{homeId}/installations` and `GET /homes/{homeId}/installations/{installationId}`, and extended the playground to display installation data for live verification.
- Added bridge and boiler-by-bridge support for `GET /bridges/{bridgeId}`, `GET /homeByBridge/{bridgeId}/boilerInfo`, `GET|PUT /homeByBridge/{bridgeId}/boilerMaxOutputTemperature`, and `GET /homeByBridge/{bridgeId}/boilerWiringInstallationState`.
- Added invitation support for `GET /homes/{homeId}/invitations`, `POST /homes/{homeId}/invitations`, `DELETE /homes/{homeId}/invitations/{invitationToken}`, and `POST /homes/{homeId}/invitations/{invitationToken}/resend`.

### Fixed
- Corrected zone overlay cleanup to accept the Tado API's `204 No Content` response for `DELETE /homes/{homeId}/zones/{zoneId}/overlay`, which fixes the live integration round-trip path.
- Added focused tests for invitation list, create, delete, resend, and invitation payload validation paths.
- Added focused tests covering the remaining overlay command payload variants, including Fahrenheit, hot-water, off, and default wrapper flows.

### Changed
- Promoted the SDK to the `1.0.0` release.
- Updated the playground to auto-detect the Internet Bridge serial, prompt for the printed bridge auth key when needed, and use consistent section-banner formatting for live diagnostics.

## [0.9.0] - 2026-04-07

### Added
- Added zone schedule/timetable operations for `GET|PUT /homes/{homeId}/zones/{zoneId}/schedule/activeTimetable`, `GET /homes/{homeId}/zones/{zoneId}/schedule/timetables`, `GET /homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}`, `GET /homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}/blocks`, and `GET|PUT /homes/{homeId}/zones/{zoneId}/schedule/timetables/{timetableTypeId}/blocks/{dayType}`.
- Updated the playground to show each zone's active schedule days and a simple timetable table.

### Fixed
- Added focused tests for timetable read and write routes, mapped response handling, and timetable payload validation.
- Added the remaining `ZoneAppService` timetable passthrough coverage and reduced expected `404 NotFound` SDK noise in the playground/logging pipeline.
- Standardised the runtime JSON stack on `System.Text.Json` by removing the unused `Newtonsoft.Json` dependency and aligning the README with the actual serializer implementation.
- Cleaned up nullable reference warnings across core entities, response DTOs, overlay handling, and test HTTP mocks so the solution builds warning-free.

### Changed
- Bumped the package and assembly version metadata to `0.9.0`.
- Removed the unused username/password configuration contract from `TadoApiConfig` and aligned authentication guidance around OAuth2 device authorization.
- Added a parameterless `AddTadoInfrastructure()` overload for the default device-flow setup and updated the playground to use the transport-only config shape.
- Tightened CI so push and pull-request runs execute build plus unit tests, while live integration tests run through a separate manual workflow using `TADO_ACCESS_TOKEN`, `TADO_HOME_ID`, and `TADO_HEATING_ZONE_ID`.

## [0.8.8] - 2026-04-07

### Added
- Added `SetAwayRadiusInMetersAsync` for `PUT /homes/{homeId}/awayRadiusInMeters`.
- Added `SetIncidentDetectionAsync` for `PUT /homes/{homeId}/incidentDetection`.
- Added `SetHomeDetailsAsync` for `PUT /homes/{homeId}/details` using the complete writable home-details payload.
- Added `SetFlowTemperatureOptimisationAsync` for `PUT /homes/{homeId}/flowTemperatureOptimization`.
- Added `MoveDeviceToZoneAsync` for `POST /homes/{homeId}/zones/{zoneId}/devices` with support for the optional `force` query flag.
- Added `SetZoneMeasuringDeviceAsync` for `PUT /homes/{homeId}/zones/{zoneId}/measuringDevice`.
- Added `SetOpenWindowDetectionAsync` for `PUT /homes/{homeId}/zones/{zoneId}/openWindowDetection`, including the spec-required `roomId`, `enabled`, and `timeoutInSeconds` payload.
- Added `ActivateOpenWindowAsync` for `POST /homes/{homeId}/zones/{zoneId}/state/openWindow/activate`.
- Added `ResetOpenWindowAsync` for `DELETE /homes/{homeId}/zones/{zoneId}/state/openWindow`.
- Added `SetZoneDetailsAsync` for `PUT /homes/{homeId}/zones/{zoneId}/details` using the writable zone-details payload.
- Added `SetDefaultZoneOverlayAsync` for `PUT /homes/{homeId}/zones/{zoneId}/defaultOverlay` using the writable default-overlay payload.
- Added `SetZoneOverlaysAsync` for `POST /homes/{homeId}/overlay` using the spec-defined bulk overlay payload.
- Added `DeleteZoneOverlaysAsync` for `DELETE /homes/{homeId}/overlay` using repeated `rooms` query parameters.
- Added `SetAwayConfigurationAsync` for `PUT /homes/{homeId}/zones/{zoneId}/schedule/awayConfiguration` using the writable away-configuration payload.
- Added `SetZoneTemperatureOffsetFahrenheitAsync` for `PUT /devices/{deviceId}/temperatureOffset` using the spec-supported `fahrenheit` payload.

### Fixed
- Added focused tests for the `SetAwayRadiusInMetersAsync` command route, `204` status expectation, payload shape, and negative-distance validation.
- Added focused tests for the `SetIncidentDetectionAsync` command route and serialized `enabled` payload shape.
- Added focused tests for the `SetHomeDetailsAsync` command route, serialized payload shape, and required-name validation.
- Added focused tests for the `SetFlowTemperatureOptimisationAsync` command route and serialized `maxFlowTemperature` payload shape.
- Added focused tests for the `MoveDeviceToZoneAsync` command route, HTTP method, expected status code, payload shape, and serial validation.
- Added focused tests for the `SetZoneMeasuringDeviceAsync` command route, typed response handling, payload shape, and serial validation.
- Added focused tests for the `SetOpenWindowDetectionAsync` command route, serialized payload shape, and required-setting validation.
- Added focused tests for the `ActivateOpenWindowAsync` command route and `204` status expectation.
- Added focused tests for the `ResetOpenWindowAsync` command route and `204` status expectation.
- Added focused tests for the `SetZoneDetailsAsync` command route, mapped response handling, and required-name validation.
- Added focused tests for the `SetDefaultZoneOverlayAsync` command route, mapped response handling, and termination validation.
- Added focused tests for the bulk overlay command routes, serialized payload shape, and zone-id validation.
- Added focused tests for the `SetAwayConfigurationAsync` command route, serialized payload shape, and required-setting validation.
- Added focused tests for the `SetZoneTemperatureOffsetFahrenheitAsync` command route and serialized `fahrenheit` payload shape.

### Changed
- Bumped the package and assembly version metadata to `0.8.8`.

## [0.8.7] - 2026-04-05

### Added
- Added `SetHeatingCircuitAsync` for `PUT /homes/{homeId}/zones/{zoneId}/control/heatingCircuit`, including support for clearing the assignment by sending an empty body.

### Fixed
- Added focused tests for the `SetHeatingCircuitAsync` command route, payload shape, null-body removal behavior, and circuit-number validation.

## [0.8.6] - 2026-04-05

### Added
- Added `SetHeatingTemperatureFahrenheitAsync`, `SetHotWaterTemperatureCelsiusAsync`, `SetHotWaterTemperatureFahrenheitAsync`, `SwitchHeatingOffAsync`, and `SwitchHotWaterOffAsync` for `PUT /homes/{homeId}/zones/{zoneId}/overlay`.

### Fixed
- Added focused tests for the new overlay command wrappers to verify device type, power state, and Celsius/Fahrenheit payload shape.

## [0.8.5] - 2026-04-05

### Added
- Added `SetMobileDeviceSettingsAsync` for `PUT /homes/{homeId}/mobileDevices/{mobileDeviceId}/settings`.

### Fixed
- Added focused tests for the `SetMobileDeviceSettingsAsync` command route, HTTP method, payload shape, and null-settings validation.

## [0.8.4] - 2026-04-05

### Added
- Added `DeleteZoneOverlayAsync` for `DELETE /homes/{homeId}/zones/{zoneId}/overlay`.
- Added `DeleteMobileDeviceAsync` for `DELETE /homes/{homeId}/mobileDevices/{mobileDeviceId}`.

### Fixed
- Added focused tests for the `DeleteZoneOverlayAsync` and `DeleteMobileDeviceAsync` command routes, HTTP methods, and expected status codes.

## [0.8.3] - 2026-04-05

### Added
- Added `CreateZoneAsync` for `POST /homes/{homeId}/zones` with support for the optional `force` query flag.

### Fixed
- Added focused tests for the `CreateZoneAsync` command route, HTTP method, expected status code, and payload shape.
- Completed XML parameter and return documentation across the public application, domain, and infrastructure service surfaces for fuller IntelliSense.

## [0.8.2] - 2026-04-05

### Added
- Added `ResetHomePresenceAsync` for `DELETE /homes/{homeId}/presenceLock`.

### Changed
- Aligned `SetHeatingTemperatureCelsiusAsync` overlay requests with the upstream `termination.typeSkillBasedApp` contract.

### Fixed
- Added focused tests for the `ResetHomePresenceAsync` delete command route, HTTP method, and expected status code.
- Strengthened `SetHeatingTemperatureCelsiusAsync` command tests to assert the serialized overlay payload shape.

## [0.8.1] - 2026-04-05

### Changed
- Aligned `SetHomePresenceAsync` with the upstream `PUT /homes/{homeId}/presenceLock` contract and `homePresence` request payload.

### Fixed
- Added focused tests for the `SetHomePresenceAsync` command route, HTTP method, expected status code, and payload shape.

## [0.8.0] - 2026-04-05

### Changed
- Aligned `GetDeviceAsync` with the upstream `GET /devices/{deviceId}` route while keeping the legacy numeric overload as a compatibility wrapper.

### Fixed
- Added exact-path tests for `GetDeviceAsync` so route mismatches are caught in the test suite.

## [0.7.0] - 2026-04-05

### Added
- Centralised solution-wide version metadata via `Directory.Build.props`.

### Changed
- Added a short README pointer to `docs/sdk-surface.yaml` near the top of the project overview.

### Fixed
- Added richer throttling diagnostics, including `RequestThrottledException` and README documentation for throttling behaviour.
