# Change Log

All notable changes to "RainLisp" will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/).

Semantic versioning is followed.

## [Unreleased]

### Added

### Fixed

### Changed

### Removed

## [2.0.0] - 2025-12-14

### Changed
- Target .NET 8.

## [1.5.0] - 2025-06-10

### Added
- `at-list` library procedure that returns a list item at a given position.
- `at-stream` library procedure that returns a stream item at a given position.
- A tutorial section about streams.

### Changed
- Memoized user procedures involved in streams are printed to the standard
output as such, instead of just user procedures.

## [1.4.0] - 2025-02-16

### Added
- `delay` special form for supporting delayed expressions.
- `force` library procedure to force the evaluation of a delayed expression.
- `cons-stream` derived expression for creating infinite streams.
- `cdr-stream` library procedure to enable walking through a stream.
- `make-range-stream` library procedure to make a numerical range stream.
- `map-stream` library procedure that projects elements of a stream.
- `filter-stream` library procedure that filters elements of a stream.

## [1.3.0] - 2024-06-30

### Added
- `parse-number-culture` primitive procedure. #146

## [1.2.0] - 2024-02-18

### Added
- Add extension methods in the `RainLisp.DotNetIntegration` namespace for easier consumption by .NET systems.

## [1.1.0] - 2024-01-28

### Added
- `ceiling` and `floor` primitive procedures.

## [1.0.0] - 2023-07-20

Initial release.

## [1.0.0-beta.1] - 2023-07-11

Initial beta release.
