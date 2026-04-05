# Changelog

All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog, and this project follows Semantic Versioning.

## [Unreleased]

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
