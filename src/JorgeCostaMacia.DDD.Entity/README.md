# JorgeCostaMacia.DDD.Entity

Marker interface (`IEntity`) for **identity-based domain entities** — objects defined by a stable identity across state changes, the DDD Entity building block. **Zero dependencies.**

[![NuGet](https://img.shields.io/nuget/v/JorgeCostaMacia.DDD.Entity.svg)](https://www.nuget.org/packages/JorgeCostaMacia.DDD.Entity/)
[![Downloads](https://img.shields.io/nuget/dt/JorgeCostaMacia.DDD.Entity.svg)](https://www.nuget.org/packages/JorgeCostaMacia.DDD.Entity/)
[![Build](https://github.com/JorgeCostaMacia/ddd-dotnet/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/JorgeCostaMacia/ddd-dotnet/actions/workflows/main.yml)
[![License](https://img.shields.io/github/license/JorgeCostaMacia/ddd-dotnet.svg)](https://github.com/JorgeCostaMacia/ddd-dotnet/blob/main/LICENSE.txt)

---

## Install

```bash
dotnet add package JorgeCostaMacia.DDD.Entity
```

## Usage

```csharp
using JorgeCostaMacia.DDD.Entity.Domain;

public class Customer : IEntity
{
    public Guid Id { get; init; }
    // identity-based: two Customers are equal when their Id is equal
}
```

An **Entity** is defined by its identity and lifecycle (not its attributes), and is mutable — unlike a Value Object.

## Requirements

The **.NET 10** SDK.

## About

`JorgeCostaMacia.DDD.Entity` is part of **[ddd-dotnet](https://github.com/JorgeCostaMacia/ddd-dotnet)** — DDD building blocks for .NET: value objects with their FluentValidation validators, aggregates, entities, domain events and the domain exception hierarchy, each package scoped to a single concern.

- **Repository:** [github.com/JorgeCostaMacia/ddd-dotnet](https://github.com/JorgeCostaMacia/ddd-dotnet)
- **Issues & requests:** [open an issue](https://github.com/JorgeCostaMacia/ddd-dotnet/issues)
- **Contributing:** [CONTRIBUTING.md](https://github.com/JorgeCostaMacia/ddd-dotnet/blob/main/CONTRIBUTING.md)
- **Security:** [report a vulnerability](https://github.com/JorgeCostaMacia/ddd-dotnet/security/advisories/new)

**Author:** Jorge Costa Maciá

- [LinkedIn](https://www.linkedin.com/in/jorge-costa-macia-842817164/)
- [GitHub](https://github.com/JorgeCostaMacia/)
- [Bitbucket](https://bitbucket.org/jorgecostamacia/)
