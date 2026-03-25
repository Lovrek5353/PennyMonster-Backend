# PennyMonster Backend API 👾💳

## Overview
PennyMonster is a personal finance application originally developed for my Master’s thesis using Firebase/Firestore. I am currently rebuilding the backend from scratch as a custom **ASP.NET Core REST API** backed by **SQL Server**. 

This repository serves as my hands-on sandbox to master the .NET ecosystem. The primary engineering goal is to transition from a loosely structured NoSQL BaaS to a highly controlled, relational backend. Because financial applications rely on strict mathematical consistency (e.g., balance tracking, loan amortization), this rewrite focuses heavily on enforcing data integrity, relational constraints, and predictable API contracts.

> **Note:** This project is actively a work in progress. Core architectural pillars are in place, and I am continually adding new features and refinements.

---

## 🏗️ Architecture & Core Patterns



Rather than building a basic CRUD application, I am treating this as a practice ground for production-grade architecture:

* **Clean Architecture & Service Layer:** Controllers are kept intentionally thin. All complex workflows, data aggregation, and entity manipulations are delegated to an injected Service layer.
* **Rich Domain Models (DDD):** Moving away from "anemic" data models. State transitions—such as a Saving Pocket automatically marking itself as "Completed" when the target balance is reached—are encapsulated directly within the entities to protect invariants and avoid "God objects" in the services.
* **Constrained Querying:** Leveraging `IQueryable` for deferred execution. Pagination and dynamic filtering are pushed directly to the SQL database to minimize memory overhead, but these expression trees are strictly bounded within the Service layer so they do not leak into API Controllers.
* **Global Exception Handling:** A custom middleware pipeline intercepts all unhandled exceptions. It maps custom domain rules (`BusinessRuleException`) to proper HTTP 400-level codes and returns standardized **RFC 7807 ProblemDetails** to the frontend, ensuring stack traces are never leaked.
* **Application-Level Tenant Isolation:** API endpoints are secured via JWT. Cross-tenant data leaks are prevented by extracting the `UserId` directly from the auth claims and injecting it into the service layer queries.

---

## ⚙️ Functionality (What it does)

The API manages the core financial entities required for personal wealth tracking:
* **Transactions:** Full CRUD for logging income and expenses with categorical filtering.
* **Saving Pockets:** Goal-oriented savings trackers that dynamically calculate their status based on deadlines and current balances.
* **Tabs (Loans/Debts):** Tracking money owed to or by the user, with strict rules preventing the cancellation of active or completed tabs.
* **User Authentication:** Currently configured to trust and integrate with third-party identity providers (e.g., Firebase Auth) via JWT validation.

---

## 💻 Tech Stack

* **Framework:** .NET 10
* **Web Framework:** ASP.NET Core Web API
* **ORM:** Entity Framework Core (EF Core)
* **Database:** Microsoft SQL Server
* **Validation:** FluentValidation
* **Authentication:** JWT Bearer Authentication

---

## 🚀 Roadmap / What's Next

Because this is an active learning environment, I am progressively layering in new enterprise features. Currently on the roadmap:

- [ ] **Structured Logging & Observability:** Integrating Serilog to write structured JSON logs to files for better debugging and tracing.
- [ ] **API Rate Limiting:** Protecting endpoints from abuse using .NET's built-in rate limiting middleware.
- [ ] **Response Caching:** Implementing caching for frequently requested, rarely changing data (like Categories).
- [ ] **Unit & Integration Testing:** Setting up xUnit and Moq to test domain logic invariants and service layer orchestration.
