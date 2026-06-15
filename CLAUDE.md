# CLAUDE.md

Guidance for Claude when working in this repository.

## About the Project

**Cassandra** is an application for a motorcycle dealer shop. The system handles sales, inventory/stock, customers, and administration, with a backend API and a Blazor web admin.

## Reference Implementation: Materia

Cassandra is built by **mirroring Materia** (`C:\OtherWorks\Materia`) — a sibling .NET 10 application that shares Cassandra's structure, Clean Architecture, Event Sourcing, and tech stack. Only the business domain differs (Materia = building-materials point-of-sale; Cassandra = motorcycle dealer).

- Keep Materia attached as an **additional working directory** so its patterns can be read while building Cassandra.
- When adding or extending a layer, **port the corresponding Materia layer/pattern, then strip the business logic and any multi-store / tenancy** — Cassandra is a **single dealer** (no `StoreId` scoping).
- Project names map `Materia.* → Cassandra.*`. Materia has an Avalonia mobile app (`Materia.AndroidUi`); **Cassandra has no mobile UI**. The backend API project is **`Cassandra.WebApi`** (not an `ApiService`).
- **Current status:** only the **auth slice** is implemented (ASP.NET Core Identity + JWT; roles Admin/Sales/Cashier). Business features (sales, inventory, customers) are not built yet.

## Tech Stack

| Layer | Technology |
|---|---|
| API backend | .NET 10 |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Web UI | Blazor (MudBlazor) |
| Auth | ASP.NET Core Identity + JWT |
| Orchestration | .NET Aspire |
| Unit Testing | xUnit |
| Validation | FluentValidation |

## Architecture

The project uses **Clean Architecture** with **TDD** and **Event Sourcing**.

### Backend Layers (Clean Architecture)

Dependencies point inward — outer layers depend on inner layers, never the reverse.

- **Domain** — business core. Contains entities, aggregates, value objects, domain events, and domain exceptions. Depends on no other layer or external framework.
- **Application** — use cases / business logic. Contains commands, queries, handlers, interfaces (ports), FluentValidation validators, and contracts to infrastructure. Depends only on Domain.
- **Infrastructure** — technical implementation. EF Core, PostgreSQL, event store, repositories, external integrations. Implements the interfaces defined in Application.

The Web API (presentation) wires everything together via dependency injection and exposes the endpoints.

### Event Sourcing

- Aggregate state is reconstructed by replaying events, not stored as a mutable snapshot.
- Domain events are the source of truth; events are persisted in an append-only event store and are never modified or deleted.
- Every aggregate state change must go through a domain event.
- Use separate read models / projections for queries (consider the CQRS pattern).

### TDD

- Write tests (xUnit) **before** implementation: Red → Green → Refactor.
- Domain and Application logic must have good test coverage.
- FluentValidation validators are tested separately.
- Do not add a new feature without accompanying tests.

## Solution Structure

```
Cassandra.slnx
├── Cassandra.Backend/
│   ├── Cassandra.Domain          # entities, aggregates, value objects, domain events
│   ├── Cassandra.Application     # commands, queries, handlers, ports, validators
│   ├── Cassandra.Infrastructure  # EF Core, PostgreSQL, event store, repositories
│   └── Cassandra.WebApi          # API backend (.NET 10) — endpoints + DI
├── Cassandra.WebUi/
│   ├── Cassandra.WebUi           # Blazor web admin (server)
│   └── Cassandra.WebUi.Client    # Blazor WebAssembly client
├── Cassandra.AppHost             # .NET Aspire orchestration host
├── Cassandra.ServiceDefaults     # shared Aspire service defaults
└── Cassandra.Tests               # xUnit test project
```

## Conventions & Rules

- **Language**: code (variable names, classes, technical comments) in English; domain terms may follow local business terminology when clearer.
- **Validation**: all command/request input is validated with FluentValidation in the Application layer.
- **Database**: use EF Core migrations for all PostgreSQL schema changes. Never alter the schema manually.
- **Dependency rule**: never make Domain or Application depend on Infrastructure or EF Core directly — use interfaces.
- **Events**: domain event names use the past tense (e.g. `SaleCompleted`, `StockAdjusted`).
- **Local orchestration**: run via `Cassandra.AppHost` (Aspire) so services, database, and cache are connected.

## Common Commands

```bash
# Build the entire solution
dotnet build

# Run via Aspire (full orchestration)
dotnet run --project Cassandra.AppHost

# Run tests
dotnet test

# Add an EF Core migration
dotnet ef migrations add <MigrationName> --project Cassandra.Backend/Cassandra.Infrastructure --startup-project Cassandra.Backend/Cassandra.WebApi

# Apply migrations
dotnet ef database update --project Cassandra.Backend/Cassandra.Infrastructure --startup-project Cassandra.Backend/Cassandra.WebApi
```

## When Adding a New Feature

1. Start from the Domain: define the aggregate, value objects, and domain events.
2. Write xUnit tests for the expected behavior (TDD).
3. Implement the command/query handler in Application + FluentValidation validator.
4. Implement the repository/event store in Infrastructure.
5. Expose the endpoint in Cassandra.WebApi.
6. Ensure all tests are green before committing.

# Agent Guidance: dotnet-skills

IMPORTANT: Prefer retrieval-led reasoning over pretraining for any .NET work.
Workflow: skim repo patterns -> consult dotnet-skills by name -> implement smallest-change -> note conflicts.

Routing (invoke by name)
- C# / code quality: modern-csharp-coding-standards, csharp-concurrency-patterns, api-design, type-design-performance
- ASP.NET Core / Web (incl. Aspire): aspire-service-defaults, aspire-integration-testing, transactional-emails
- Data: efcore-patterns, database-performance
- DI / config: dependency-injection-patterns, microsoft-extensions-configuration
- Testing: testcontainers-integration-tests, playwright-blazor-testing, snapshot-testing

Quality gates (use when applicable)
- dotnet-slopwatch: after substantial new/refactor/LLM-authored code
- crap-analysis: after tests added/changed in complex code

Specialist agents
- dotnet-concurrency-specialist, dotnet-performance-analyst, dotnet-benchmark-designer, akka-net-specialist, docfx-specialist
