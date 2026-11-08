# JorgeCostaMacia.DDD.DomainEvent

Marker interface (`IDomainEvent`) for a **transport-agnostic domain event** — an immutable fact about something that happened in the domain, raised by aggregates. **Zero dependencies.**

[![NuGet](https://img.shields.io/nuget/v/JorgeCostaMacia.DDD.DomainEvent.svg)](https://www.nuget.org/packages/JorgeCostaMacia.DDD.DomainEvent/)
[![Downloads](https://img.shields.io/nuget/dt/JorgeCostaMacia.DDD.DomainEvent.svg)](https://www.nuget.org/packages/JorgeCostaMacia.DDD.DomainEvent/)
[![Build](https://github.com/JorgeCostaMacia/ddd-dotnet/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/JorgeCostaMacia/ddd-dotnet/actions/workflows/main.yml)
[![License](https://img.shields.io/github/license/JorgeCostaMacia/ddd-dotnet.svg)](https://github.com/JorgeCostaMacia/ddd-dotnet/blob/main/LICENSE.txt)

---

## Install

```bash
dotnet add package JorgeCostaMacia.DDD.DomainEvent
```

## Usage

```csharp
using JorgeCostaMacia.DDD.DomainEvent.Domain;

public record OrderCreated(Guid OrderId) : IDomainEvent;
```

The domain depends only on this marker — it doesn't know how (or whether) the event travels over a bus. A messaging layer can specialize it (`IEvent : IDomainEvent, IMessage, ...`) so concrete events fit an aggregate's event list while the domain stays transport-free.

## Requirements

The **.NET 10** SDK.

## About

`JorgeCostaMacia.DDD.DomainEvent` is part of **[ddd-dotnet](https://github.com/JorgeCostaMacia/ddd-dotnet)** — DDD building blocks for .NET: value objects with their FluentValidation validators, aggregates, entities, domain events and the domain exception hierarchy, each package scoped to a single concern.

- **Repository:** [github.com/JorgeCostaMacia/ddd-dotnet](https://github.com/JorgeCostaMacia/ddd-dotnet)
- **Issues & requests:** [open an issue](https://github.com/JorgeCostaMacia/ddd-dotnet/issues)
- **Contributing:** [CONTRIBUTING.md](https://github.com/JorgeCostaMacia/ddd-dotnet/blob/main/CONTRIBUTING.md)
- **Security:** [report a vulnerability](https://github.com/JorgeCostaMacia/ddd-dotnet/security/advisories/new)

**Author:** Jorge Costa Maciá

- [LinkedIn](https://www.linkedin.com/in/jorge-costa-macia-842817164/)
- [GitHub](https://github.com/JorgeCostaMacia/)
- [Bitbucket](https://bitbucket.org/jorgecostamacia/)
