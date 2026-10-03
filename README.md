# nopCommerce.Modern: Legacy .NET Framework 4.5.1 Monolith to .NET 8 Clean Architecture

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build & Test](https://img.shields.io/badge/tests-9%20passed-brightgreen.svg)]()

Modernized enterprise architecture port of **nopCommerce** legacy business logic (Products, Categories, Inventory) from .NET Framework 4.5.1 / Autofac / EF6 to **ASP.NET Core 8 WebAPI and Entity Framework Core 8**.

---

## Architecture Transformation

| Legacy nopCommerce Component | Modern .NET 8 Replacement | Architectural Benefits |
|---|---|---|
| .NET Framework 4.5.1 / Windows-only IIS | Cross-platform .NET 8 (Linux, macOS, Windows) | Runs on Docker, Kubernetes, Alpine Linux |
| Autofac XML / ContainerBuilder | Native Microsoft.Extensions.DependencyInjection | High-performance constructor injection, zero external IoC bloat |
| Entity Framework 6 `IDbContext` | Entity Framework Core 8 `NopDbContext` | Compiled queries, async execution, batch updates |
| Monolithic MVC Razor views | Headless E-Commerce WebAPI (`/api/products`) | Decoupled frontend (Next.js, Vue, Blazor, mobile) |

---

## Solution Structure

```
nopCommerceModern/
├── Nop.Modern.sln
├── Nop.Modern.Api/
│   ├── Controllers/ProductsController.cs     # E-commerce REST endpoints
│   ├── Data/NopDbContext.cs                 # EF Core data context
│   ├── Models/ProductModels.cs              # Product, Category, Manufacturer entities
│   ├── Services/ProductService.cs           # Business logic & pricing calculations
│   └── Program.cs                           # App bootstrapping & OpenAPI setup
└── Nop.Modern.Tests/
    └── ProductServiceTests.cs               # xUnit unit test coverage
```

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build & Run Tests
```bash
dotnet test
```

### Run WebAPI Server
```bash
dotnet run --project Nop.Modern.Api
```
Explore endpoints at `http://localhost:5000/api/products`.
