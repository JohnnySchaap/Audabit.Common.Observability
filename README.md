# Audabit.Common.Observability

A structured logging and event emission library providing the `IEmitter<T>` pattern for type-safe observability.

## Why You Should Use Structured Logging

Unstructured log strings make troubleshooting painful. Searching through logs for "User created" returns thousands of matches with no way to filter by user ID or timestamp. When production issues occur, you waste hours parsing text instead of querying data.

### Queryable Event Data

Structured logging turns log messages into queryable data. Each event contains strongly-typed properties that log aggregation systems can index and search. Find all failed user creations for a specific user ID in seconds instead of grep-ing through gigabytes of text files.

### Type-Safe Event Definitions

This library enforces type safety at compile time. Create event classes with strongly-typed properties instead of string formatting log messages. Refactoring becomes safe - the compiler catches when you remove or rename properties that events depend on.

### Category-Based Logger Injection

The `IEmitter<T>` pattern automatically categorizes logs by the class that emits them. Instead of passing logger names as strings, use generics. Your service class gets a logger scoped to that service automatically, making it easy to filter logs by component.

### Consistent Service Context

Set the service name once at startup and every event includes it automatically. When running multiple microservices, logs are tagged with the originating service without manual configuration in every class. This enables distributed tracing and correlation across services.

## Dependencies

This library has no dependencies on other Audabit packages and is fully standalone.

## Library Design Principles

> **ConfigureAwait Best Practices**: This library follows Microsoft's recommended async/await best practices by using `ConfigureAwait(false)` on all await statements. This eliminates unnecessary context switches and improves performance by allowing continuations to run on any thread pool thread rather than marshaling back to the original synchronization context.

## Features

- **Type-Safe Event Emission**: Generic `IEmitter<T>` for logger categorization per class
- **Structured Logging**: Events with strongly-typed properties for queryable logs
- **Event Abstractions**: Base classes for `LoggingEvent` and `ErrorEvent`
- **Service Name Management**: Thread-safe service name configuration
- **Framework-Agnostic**: Core library works with any .NET application
- **.NET 10.0 Support**: Built for the latest .NET framework

## Installation

### Via .NET CLI

```bash
dotnet add package Audabit.Common.Observability
```

### Via Package Manager Console

```powershell
Install-Package Audabit.Common.Observability
```

## Getting Started

### Creating Events

```csharp
using Audabit.Common.Observability.Events;

[ExcludeFromCodeCoverage]
public class UserCreatedEvent : LoggingEvent
{
    public UserCreatedEvent(string userId, string username)
        : base(nameof(UserCreatedEvent))
    {
        Properties.Add(nameof(userId), userId);
        Properties.Add(nameof(username), username);
    }
}

[ExcludeFromCodeCoverage]
public class UserCreationFailedEvent : ErrorEvent
{
    public UserCreationFailedEvent(Exception exception, string username)
        : base(
            nameof(UserCreationFailedEvent),
            exception?.Message ?? string.Empty,
            exception: exception,
            properties: null)
    {
        Properties.Add(nameof(username), username);
    }
}
```

### Using IEmitter

```csharp
using Audabit.Common.Observability.Emitters;

public class UserService(IEmitter<UserService> emitter)
{
    public void CreateUser(string username)
    {
        try
        {
            emitter.RaiseInformation(new UserCreatedEvent(Guid.NewGuid().ToString(), username));
            // Business logic
        }
        catch (Exception ex)
        {
            emitter.RaiseError(new UserCreationFailedEvent(ex, username));
            throw;
        }
    }
}
```

## How It Works

The observability library provides a structured approach to logging and event emission:

1. **Event Definition**: Create strongly-typed event classes inheriting from `LoggingEvent` or `ErrorEvent`
2. **Service Registration**: The `IEmitter<T>` is injected into your services via dependency injection
3. **Type-Safe Logging**: Each service gets its own logger category based on the generic type parameter
4. **Structured Properties**: Events contain structured data that can be queried in log aggregation systems
5. **Service Context**: The service name is automatically included in all emitted events for distributed tracing

The `IEmitter<T>` interface provides methods for different log levels:
- `RaiseInformation(LoggingEvent)` - Informational events
- `RaiseWarning(LoggingEvent)` - Warning events  
- `RaiseError(ErrorEvent)` - Error events with exception details

## Related Packages

This library works seamlessly with other Audabit packages:

- **[Audabit.Common.Observability.AspNet](https://dev.azure.com/johnnyschaap/Audabit/_artifacts/feed/Audabit/NuGet/Audabit.Common.Observability.AspNet)**: ASP.NET Core integration with automatic service registration and JSON console logging
- **[Audabit.Common.CorrelationId.AspNet](https://dev.azure.com/johnnyschaap/Audabit/_artifacts/feed/Audabit/NuGet/Audabit.Common.CorrelationId.AspNet)**: Adds correlation ID tracking to structured logs for distributed tracing
- **[Audabit.Common.ExceptionHandling.AspNet](https://dev.azure.com/johnnyschaap/Audabit/_artifacts/feed/Audabit/NuGet/Audabit.Common.ExceptionHandling.AspNet)**: Global exception handling with structured error logging

This package is the core observability library. For ASP.NET Core applications, use Audabit.Common.Observability.AspNet which provides automatic configuration and service registration.

## Build and Test

### Prerequisites

- .NET 10.0 SDK or later
- Visual Studio 2022 / VS Code / Rider (optional)

### Building

```bash
dotnet restore
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Creating NuGet Package

```bash
dotnet pack --configuration Release
```

## CI/CD Pipeline

This project uses Azure DevOps pipelines with the following features:

- **Automatic Versioning**: Major and minor versions from csproj, patch version from build number
- **Prerelease Builds**: Non-main branches create prerelease packages (e.g., `9.0.123-feature-auth`)
- **Code Formatting**: Enforces `dotnet format` standards
- **Code Coverage**: Generates and publishes code coverage reports
- **Automated Publishing**: Pushes packages to Azure Artifacts feed

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Guidelines

1. Follow existing code style and conventions
2. Ensure all tests pass before submitting PR
3. Add tests for new features
4. Update documentation as needed
5. Run `dotnet format` before committing

## License

Copyright © Audabit Software Solutions B.V. 2026

Licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE) file for details.

## Authors

- [John Schaap](https://github.com/JohnnySchaap) - [Audabit Software Solutions B.V.](https://audabit.nl)

## Acknowledgments

- Designed for .NET Core and .NET applications
- Companion library: Audabit.Common.Observability.AspNet for ASP.NET Core integration
