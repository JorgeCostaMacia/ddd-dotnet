# JorgeCostaMacia.DDD.Exception

A small hierarchy of **domain exceptions** carrying traceable metadata — a stable id, type name, error code, HTTP status code and UTC timestamp — for consistent logging, correlation and API error mapping.

[![NuGet](https://img.shields.io/nuget/v/JorgeCostaMacia.DDD.Exception.svg)](https://www.nuget.org/packages/JorgeCostaMacia.DDD.Exception/)
[![Downloads](https://img.shields.io/nuget/dt/JorgeCostaMacia.DDD.Exception.svg)](https://www.nuget.org/packages/JorgeCostaMacia.DDD.Exception/)
[![Build](https://github.com/JorgeCostaMacia/ddd-dotnet/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/JorgeCostaMacia/ddd-dotnet/actions/workflows/main.yml)
[![License](https://img.shields.io/github/license/JorgeCostaMacia/ddd-dotnet.svg)](https://github.com/JorgeCostaMacia/ddd-dotnet/blob/main/LICENSE.txt)

---

## Install

```bash
dotnet add package JorgeCostaMacia.DDD.Exception
```

## Types

All are `abstract` — inherit them in your bounded context to create concrete, named exceptions.

| Type | For | Default HTTP |
| --- | --- | --- |
| `DomainException` | base of the hierarchy | 500 |
| `ErrorException` | one or more general business errors (`Errors`) | 400 |
| `ExistException` | resource already exists / conflict | 409 |
| `NotFoundException` | resource not found | 404 |
| `ValidationException` | validation failures (`Validations`) | 400 |

## Usage

```csharp
using JorgeCostaMacia.DDD.Exception.Domain;

public sealed class CustomerNotFoundException(Guid customerId)
    : NotFoundException(null, null, null, null, null, $"Customer {customerId} was not found.", null);

// every null falls back to a default: AggregateId (UUIDv7),
// AggregateType (the NotFoundException full name), AggregateCode,
// AggregateHttpCode (404), AggregateOccurredAt (UTC now).
```

`ValidationException` carries a list of `FluentValidation.Results.ValidationFailure`, so it plugs straight into a FluentValidation `AbstractValidator`:

```csharp
protected override void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
    => throw new CustomerValidationException(result.Errors);
```

## Requirements

The **.NET 10** SDK.

Depends on [FluentValidation](https://www.nuget.org/packages/FluentValidation/).

## About

`JorgeCostaMacia.DDD.Exception` is part of **[ddd-dotnet](https://github.com/JorgeCostaMacia/ddd-dotnet)** — DDD building blocks for .NET: value objects with their FluentValidation validators, aggregates, entities, domain events and the domain exception hierarchy, each package scoped to a single concern.

- **Repository:** [github.com/JorgeCostaMacia/ddd-dotnet](https://github.com/JorgeCostaMacia/ddd-dotnet)
- **Issues & requests:** [open an issue](https://github.com/JorgeCostaMacia/ddd-dotnet/issues)
- **Contributing:** [CONTRIBUTING.md](https://github.com/JorgeCostaMacia/ddd-dotnet/blob/main/CONTRIBUTING.md)
- **Security:** [report a vulnerability](https://github.com/JorgeCostaMacia/ddd-dotnet/security/advisories/new)

**Author:** Jorge Costa Maciá

- [LinkedIn](https://www.linkedin.com/in/jorge-costa-macia-842817164/)
- [GitHub](https://github.com/JorgeCostaMacia/)
- [Bitbucket](https://bitbucket.org/jorgecostamacia/)
