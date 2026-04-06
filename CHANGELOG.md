# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project follows Semantic Versioning.

## [Unreleased]

### Added
- Added `SetAwayRadiusInMetersAsync` for `PUT /homes/{homeId}/awayRadiusInMeters`.
- Added `SetIncidentDetectionAsync` for `PUT /homes/{homeId}/incidentDetection`.
- Added `SetHomeDetailsAsync` for `PUT /homes/{homeId}/details` using the complete writable home-details payload.
- Added `SetFlowTemperatureOptimisationAsync` for `PUT /homes/{homeId}/flowTemperatureOptimization`.
- Added `MoveDeviceToZoneAsync` for `POST /homes/{homeId}/zones/{zoneId}/devices` with support for the optional `force` query flag.
- Added `SetZoneMeasuringDeviceAsync` for `PUT /homes/{homeId}/zones/{zoneId}/measuringDevice`.
- Added `SetZoneTemperatureOffsetFahrenheitAsync` for `PUT /devices/{deviceId}/temperatureOffset` using the spec-supported `fahrenheit` payload.

### Fixed
- Added focused tests for the `SetAwayRadiusInMetersAsync` command route, `204` status expectation, payload shape, and negative-distance validation.
- Added focused tests for the `SetIncidentDetectionAsync` command route and serialized `enabled` payload shape.
- Added focused tests for the `SetHomeDetailsAsync` command route, serialized payload shape, and required-name validation.
- Added focused tests for the `SetFlowTemperatureOptimisationAsync` command route and serialized `maxFlowTemperature` payload shape.
- Added focused tests for the `MoveDeviceToZoneAsync` command route, HTTP method, expected status code, payload shape, and serial validation.
- Added focused tests for the `SetZoneMeasuringDeviceAsync` command route, typed response handling, payload shape, and serial validation.
- Added focused tests for the `SetZoneTemperatureOffsetFahrenheitAsync` command route and serialized `fahrenheit` payload shape.

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
