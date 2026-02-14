# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project overview

- The repository contains the `MathCore` library – a collection of applied mathematical algorithms, numeric types, and utilities targeting multiple platforms: `net10.0`, `net9.0`, `net8.0`, and `netstandard2.0`.
- The library is published as a NuGet package; GitHub Actions workflows handle CI testing and publishing to NuGet.org and GitHub Packages.
- Most comments, XML‑documentation, and human‑facing text in the codebase are in Russian.

## Style and conventions (from Copilot rules)

These rules reflect the existing `.github/copilot-instructions.md` and should be followed when generating or editing code.

### Language

- Prefer Russian for:
  - XML‑documentation (`/// <summary>Описание...</summary>`),
  - inline comments,
  - user‑facing messages in this repository.

### Documentation

- Document public types and members with XML comments only.
- Use one short sentence per tag, on a single line, without a trailing period.
- Order of XML tags:
  1. `<summary>`
  2. `<param>`
  3. `<returns>`
  4. `<exception>`
  5. `<remarks>`
  6. `<example>`
- For complex public methods, add a simple usage example inside `<example>`.

### C# syntax and modern features

- Use modern C# constructs where supported by the target frameworks:
  - file‑scoped namespaces,
  - expression‑bodied members,
  - switch expressions and pattern matching,
  - target‑typed `new`, collection expressions and collection initializers,
  - `using var` and `await using`,
  - operators `??`, `??=`, `is not`, `with`.
- Keep multi‑line control blocks with braces for readability; use expression‑bodied members mainly for short, simple logic.
- Enable `#nullable` where supported and respect nullable flow analysis.

### Naming

- Local variables: `snake_case`.
- Method parameters: `PascalCase`.
- Instance fields: `_PascalCase`.
- Static fields: `__PascalCase`.
- Constants, public types and public API members: `PascalCase`.
- Prefer English identifiers for code elements (variables, methods, classes, etc.).

### General .NET practices

- Prefer guard expressions, e.g. `ArgumentNullException.ThrowIfNull(x);` at the top of public methods.
- For operations that may fail, prefer `Try*` patterns (e.g., `TryParse`) rather than exceptions for control flow.
- When generating MVVM view‑model properties (types implementing `INotifyPropertyChanged`), follow the existing compact pattern with `Set(ref field, value)` in a single line.
- Remove unused `using` directives, and keep them sorted and grouped.

## Build, test, and run commands

All commands below assume the working directory is the repository root (`MathCore`).

### Build

- Build the main library (Debug configuration):

  ```bash
  dotnet build MathCore
  ```

- Build the main library and unit tests (mirrors CI testing workflow):

  ```bash
  dotnet build MathCore
  dotnet build Tests/MathCore.Tests
  ```

- Build Release configuration for the library and tests (similar to publish workflow):

  ```bash
  dotnet build MathCore -c Release
  dotnet build Tests/MathCore.Tests -c Release
  ```

- Windows helper scripts (wrappers around `dotnet build`/`dotnet test`):
  - `BuildAndTest.bat` – build solution in default configuration, then run tests.
  - `BuildRelease.bat` – build in `Release` and run tests.
  - `BuildAndPublish.bat` – build in `Release` and run tests as a pre‑step before publishing.

### Tests

- Run the main unit test suite (MSTest, matches CI):

  ```bash
  dotnet test Tests/MathCore.Tests
  ```

- If you have already built the projects, you can skip the build step:

  ```bash
  dotnet test Tests/MathCore.Tests --no-build
  ```

- Run tests in Release configuration (as in the publish pipeline):

  ```bash
  dotnet test Tests/MathCore.Tests -c Release --no-build
  ```

- Run a single test (or a subset) using `--filter` (MSTest):

  ```bash
  dotnet test Tests/MathCore.Tests --no-build --filter "FullyQualifiedName~Namespace.ClassName.TestMethodName"
  ```

  You can also filter by traits like category if they are defined (e.g., `--filter "TestCategory=Integration"`).

### Benchmarks and sample applications

- Run performance benchmarks (BenchmarkDotNet console app):

  ```bash
  dotnet run --project Tests/Benchmarks
  ```

- Run console test application with various MathCore experiments/demos:

  ```bash
  dotnet run --project Tests/ConsoleTest
  ```

- Run algorithm demos (console app focused on `MathCore.Algorithms`):

  ```bash
  dotnet run --project Tests/MathCore.Algorithms
  ```

- Run WPF visualization/tests (Windows only):

  ```bash
  dotnet run --project Tests/MathCore.Tests.WPF
  ```

- Run WinForms test application (Windows only):

  ```bash
  dotnet run --project Tests/WinFormsTest
  ```

### Linting / formatting

- There is no dedicated lint command in this repository.
- Code style and analyzers are governed by `.editorconfig` and project settings; respect compiler and analyzer warnings when editing code.

## Architecture and project structure

### Top level

- `MathCore/` – main class library containing most functionality.
- `Tests/` – collection of projects using the library:
  - `MathCore.Tests` – MSTest unit test project for the core library.
  - `Benchmarks` – BenchmarkDotNet console app for performance testing of selected algorithms.
  - `ConsoleTest` – console application for manual experiments and ad‑hoc checks.
  - `MathCore.Algorithms` – console app showcasing and testing high‑level algorithms and visualizations built on top of `MathCore`.
  - `MathCore.Tests.WPF` – WPF application for interactive testing/visualization (depends on `MathCore.WPF`).
  - `WinFormsTest` – Windows Forms application for manual testing.
- `.scripts/` – C# file‑based tools used by `publish-nuget.bat`:
  - `xml-xpath.cs` – evaluates XPath against XML files (used to read the library version from the `.csproj`).
  - `nuget-ver-remote.cs` – reads the latest package version from NuGet.org.
  - `nuget-ver-wait.cs` – waits for a specific package version to appear on NuGet.org.
  - `dependencies.txt` – lists sibling packages (`..\MathCore.WPF`, `..\MathCore.DSP`, `..\MathCore.AI`) whose publish scripts can be chained.
- `Docs/` – markdown documentation for selected components (e.g., CRC helpers for stream extensions and similar topics).
- CI configuration under `.github/workflows/` defines how the project is built, tested, and packed on GitHub Actions.

### MathCore library layout (high level)

The `MathCore` project is organized by feature/concern, with each top‑level folder representing a logical namespace or subsystem. Key areas include:

- **Core numeric types and algebra**
  - `Complex.cs`, `Matrix.cs` and related partials implement complex numbers and matrix algebra.
  - `Vectors`, `Values`, `Functions`, `DifferentialEquations`, `Interpolation`, `Optimization`, and `Statistic` provide core mathematical algorithms and utilities.

- **Collections and data structures**
  - `Collections` – custom collection types and helpers.
  - `Trees` – tree‑like structures and traversal utilities.
  - `Attributes` (if present) – attribute types used across the library.

- **Parsing, expressions, and text processing**
  - `MathParser` – parsing and evaluation of mathematical expressions from strings.
  - `Expressions` – expression trees and helpers (including complex‑number expressions used by `Complex`).
  - `Text` – text utilities that support parsing, formatting, and related operations.

- **I/O, serialization, and interop**
  - `IO`, `CSV`, `JSON`, and `Xml` – reading/writing and serialization helpers for common formats.
  - `Net` – networking‑related utilities.
  - `Hash` – hashing helpers (e.g., for data integrity checks).
  - `PE` – utilities for working with PE (Portable Executable) structures.

- **Infrastructure and application helpers**
  - `Extensions` – shared extension methods used across the library (e.g., parsing helpers, collection extensions).
  - `Exceptions` – custom exception types.
  - `Infrastructure`, `IoC`, `Mediator`, `Service`, `ViewModels` – building blocks for higher‑level application patterns (DI, messaging/mediator, MVVM view models, etc.), used primarily by test/demo applications and dependent libraries.
  - `Logging` – logging abstractions and helpers.
  - `Threading`, `ReactiveLINQ`, `Time` – concurrency, reactive extensions, and time‑related utilities.

- **Domain‑specific utilities**
  - `Geolocation` – geospatial and coordinate‑related helpers.
  - `Graphs` – graph data structures and algorithms.
  - `Values` and `Evaluations` – wrappers around numeric values, metrics, and evaluation helpers.

This modular organization means that new functionality should usually live in the folder/namespace that matches its domain (e.g., new interpolation algorithms in `Interpolation`, new statistical helpers in `Statistic`, etc.), reusing existing primitives like `Complex`, `Matrix`, collections, and extensions wherever possible.
