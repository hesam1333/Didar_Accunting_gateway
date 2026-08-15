# AGENTS.md

## 1. Project Goal

This repository contains the Accounting Integration Gateway.

The Gateway exposes the External Accounting API, Connector provisioning/management APIs, Agent authentication APIs, and a provider-agnostic SignalR Hub.

The Gateway has **no management UI**. Connector creation, credential generation, credential retrieval, rotation, status management, and other administrative operations are exposed only through HTTP APIs.

The Gateway must remain accounting-provider agnostic.

Provider-specific implementation belongs to the local Agent, not the Gateway.

High-level flow:

```text
External Accounting API Request
        ↓
Gateway API
        ↓
Application Command
        ↓
SignalR
        ↓
Accounting Agent
        ↓
Accounting Provider
```

## 2. Technology

The project MUST use:

```text
.NET 8
ASP.NET Core 8
SignalR
```

Do not upgrade the project to newer .NET versions unless explicitly requested.

All NuGet packages must be compatible with .NET 8.

## 3. Architecture

Use Clean Architecture.

The default solution structure is:

```text
src/
    Gateway.Api/
    Gateway.Application/
    Gateway.Domain/
    Gateway.Infrastructure/

docs/

AGENTS.md
```

### Gateway.Api

Contains HTTP and SignalR entry points.

Examples:

```text
Accounting API endpoints
Connector management API endpoints
Agent authentication endpoints
SignalR Hubs
Middleware
API configuration
Dependency injection bootstrap
```

The API layer must remain thin.

Do not put accounting business logic in controllers or endpoints.

### Gateway.Application

Contains application behavior and abstractions.

Examples:

```text
Commands
Command handlers / application services
Interfaces
Application DTOs
Connector abstractions
Command dispatcher abstractions
```

Application must not depend directly on:

```text
SignalR
Entity Framework
SQL Server
ASP.NET HttpContext
provider-specific database logic
```

Infrastructure-specific functionality must be accessed through interfaces.

### Gateway.Domain

Contains core domain concepts only when they are actually needed.

Examples:

```text
Connector
Provider
Domain enums
Domain rules
```

Do NOT create unnecessary domain abstractions merely to satisfy Clean Architecture.

A small Domain project is completely acceptable.

### Gateway.Infrastructure

Contains technical implementations.

Examples:

```text
Database persistence
Entity Framework
SignalR command transport
Connector repository implementations
Credential encryption/decryption
JWT generation
External infrastructure integrations
```

Infrastructure implements abstractions defined by Application or Domain.

## 4. Source of Truth Documents

Before implementing a feature, read the relevant documents.

The main documents are:

```text
docs/EXTERNAL_ACCOUNTING_API_CONTRACT.md
docs/GATEWAY_COMMAND_CONTRACT.md
docs/CONNECTOR_AUTHENTICATION_DESIGN.md
```

### EXTERNAL_ACCOUNTING_API_CONTRACT.md

This is the source of truth for the public External Accounting HTTP API.

Use it for:

```text
Routes
HTTP methods
Request DTOs
Response DTOs
Response envelope
Field names
Serialization behavior
Existing compatibility behavior
```

Do not invent or improve the public API contract.

If the contract contains an unusual legacy behavior, preserve it unless explicitly instructed otherwise.

### GATEWAY_COMMAND_CONTRACT.md

This is the source of truth for the internal Gateway command model.

Use it for:

```text
Endpoint → Command mapping
Command names
Command responsibilities
Command flow
Gateway / Agent boundaries
```

Do not create additional accounting commands unless required by a concrete feature.

### CONNECTOR_AUTHENTICATION_DESIGN.md

This is the source of truth for:

```text
Connector identity
ApiKey
LoginToken
AgentSecret
Credential storage
Credential encryption/decryption
Connector management APIs
Agent authentication
JWT
Provider routing
SignalR authentication
Connector resolution
```

Do not create an alternative authentication model without explicit instruction.

## 5. SignalR Rules

Gateway-to-Agent command communication MUST use SignalR.

The intended flow is:

```text
Application
    ↓
IConnectorCommandDispatcher
    ↓
Infrastructure SignalR implementation
    ↓
Accounting Agent
```

Application code must not directly depend on SignalR.

Do not inject or use `IHubContext` directly from application services.

SignalR belongs to Infrastructure / API boundaries.

Use one provider-agnostic Agent hub.

Conceptually:

```text
/hubs/accounting-agent
```

Do not create separate hubs for individual accounting providers unless explicitly requested.

The SignalR connection must be associated with a `ConnectorId`.

Conceptually:

```text
ConnectorId → SignalR Connection
```

Commands sent through SignalR must support correlation between command and result.

Every command execution must have a unique `CommandId`.

## 6. Provider Independence

The Gateway must not contain accounting-provider-specific logic.

Do not add code such as:

```text
if provider == Holoo
if provider == Mahak
if provider == Sepidar
```

inside application command behavior unless the logic is strictly related to generic routing.

The provider route may identify the configured Connector:

```text
/{provider}/v1/...
```

but accounting-specific read/write behavior belongs to the local Agent.

The Gateway must not know:

```text
provider database tables
provider SQL queries
provider internal schemas
provider invoice persistence rules
provider contact persistence rules
provider product persistence rules
```

## 7. Code Readability

Readability is the most important code-quality requirement in this repository.

Code should be easy for another developer to understand without needing to reconstruct the author's reasoning.

Prefer:

```text
simple methods
clear naming
explicit control flow
small focused classes
straightforward dependency injection
obvious abstractions
```

Avoid clever code.

Avoid unnecessary generic abstractions.

Avoid deeply nested expressions.

Avoid complex LINQ when a simple readable loop is clearer.

Avoid premature optimization.

Avoid patterns that make simple behavior difficult to follow.

A developer should be able to open a class and quickly understand:

```text
what it does
why it exists
what dependencies it has
what happens next
```

## 8. Keep the Design Simple

Do not over-engineer this project.

Clean Architecture is enough.

Do not introduce additional architectural patterns unless there is a concrete requirement.

Do NOT automatically introduce:

```text
MediatR
Event Bus
RabbitMQ
MassTransit
Event Sourcing
Generic Repository frameworks
Specification pattern
Custom CQRS frameworks
Pipeline frameworks
Complex Result frameworks
Distributed caching
Redis
Background queues
Microservices
```

unless explicitly requested or clearly required by the current task.

Do not create abstractions for hypothetical future requirements.

Implement the simplest design that correctly satisfies the current documented requirement.

## 9. File Organization

Do not place multiple classes in one `.cs` file.

Use one top-level type per file.

This applies to:

```text
class
interface
record
enum
```

Examples:

```text
Connector.cs
IConnectorRepository.cs
ConnectorRepository.cs
SaveInvoiceCommand.cs
SaveInvoiceResult.cs
IConnectorCommandDispatcher.cs
SignalRConnectorCommandDispatcher.cs
```

Do not create files such as:

```text
Models.cs
Dtos.cs
Commands.cs
Helpers.cs
```

containing many unrelated types.

Folder structure should reflect responsibility and feature.

Keep folder nesting useful but shallow.

Do not create deeply nested folder structures without a clear benefit.

## 10. Naming

Names must describe intent.

Prefer:

```text
ConnectorRepository
CredentialEncryptionService
SignalRConnectorCommandDispatcher
SaveInvoiceCommand
SearchContactsCommand
AgentAuthenticationService
```

Avoid vague names such as:

```text
Manager
Helper
Utility
Processor
HandlerBase
CommonService
GeneralService
DataManager
```

unless the name genuinely describes the responsibility.

## 11. Comments

Prefer readable code over comments.

Do not add comments that merely repeat what the code says.

Comments are useful only when they explain:

```text
why an unusual compatibility behavior exists
why a security decision exists
why a non-obvious implementation is required
```

Public methods do not require XML documentation unless explicitly requested.

## 12. Testing

Do NOT write automated tests for this project unless explicitly requested.

Do not:

```text
create test projects
create unit tests
create integration tests
add testing libraries
generate mock infrastructure
```

as part of normal feature implementation.

Compilation/build verification is still expected when modifying code.

## 13. Scope Discipline

Only perform the task explicitly requested by the user.

Do not modify unrelated code.

Do not perform opportunistic refactoring.

Do not rename unrelated classes.

Do not move unrelated files.

Do not reformat unrelated files.

Do not upgrade packages unrelated to the task.

Do not change architecture outside the requested feature.

Do not fix unrelated warnings or technical debt.

If an unrelated problem is discovered:

```text
mention it
do not change it
```

unless fixing it is required for the requested work.

## 14. Existing Code

Before creating a new abstraction or service:

1. Inspect the existing implementation.
2. Check whether an appropriate abstraction already exists.
3. Reuse existing patterns where they are clean and compatible with the architecture.
4. Avoid creating duplicate abstractions.

Do not rewrite working code simply because another implementation style is possible.

## 15. Public API Compatibility

The public External Accounting API is a compatibility boundary.

Do not casually change:

```text
routes
HTTP methods
JSON property names
request structures
response structures
error envelopes
serialization behavior
```

Always verify these against `docs/EXTERNAL_ACCOUNTING_API_CONTRACT.md` before implementing or modifying an External Accounting endpoint.

The Gateway implementation must adapt to the contract.

The contract must not be adapted to make Gateway implementation easier.

## 16. Commands

Every supported External Accounting operation maps to an explicit Application command.

Current commands are:

```text
GetBusinessInfoCommand
GetProjectsCommand

GetContactCommand
SearchContactsCommand
SaveContactCommand

GetInvoiceCommand
SaveInvoiceCommand
DeleteInvoiceCommand

GetItemCommand
SearchItemsCommand
BatchSaveItemsCommand
```

Follow `docs/GATEWAY_COMMAND_CONTRACT.md` for command semantics.

Do not silently merge multiple operations into a generic command such as `ExecuteAccountingCommand`.

The explicit command names are intentional and improve readability.

## 17. Request Flow

The preferred request flow is:

```text
HTTP Endpoint
    ↓
Authenticate / Resolve Connector
    ↓
Create Application Command
    ↓
Application
    ↓
IConnectorCommandDispatcher
    ↓
Infrastructure
    ↓
SignalR
    ↓
Agent
    ↓
Result
    ↓
HTTP Response
```

Do not bypass layers for convenience.

For example, an HTTP controller must not directly send a SignalR message.

## 18. Connector Management APIs

The Gateway has no management page or UI.

Connector administration is provided through HTTP APIs.

Management APIs may support operations such as:

```text
Create Connector
Get Connector
List Connectors
Rotate LoginToken
Rotate AgentSecret
Enable Connector
Disable Connector
Get Connector Status
```

Implement only operations explicitly requested.

Do not build frontend pages, Razor Pages, MVC Views, Blazor, React, or other UI components.

When a management response includes Connector credentials, `LoginToken` and `AgentSecret` must be decrypted and returned in the response. Do not require a separate reveal-secret endpoint.

## 19. Connector Authentication

External API authentication uses:

```text
ApiKey
LoginToken
BizDomainId
Provider route
```

Agent authentication uses:

```text
ConnectorId
AgentSecret
```

These are separate credentials and must remain separate.

Do not reuse `LoginToken` as an Agent credential.

Follow `docs/CONNECTOR_AUTHENTICATION_DESIGN.md` for all authentication implementation details.

## 20. Secrets

`LoginToken` and `AgentSecret` must always use reversible authenticated encryption.

Do NOT:

```text
hash them
store them as plaintext
replace them with one-way password storage
```

Use the encryption design defined in:

```text
docs/CONNECTOR_AUTHENTICATION_DESIGN.md
```

The required V1 algorithm is:

```text
AES-256-GCM
```

The AES master key is loaded from application configuration / `appsettings`.

A unique cryptographically secure nonce must be generated for every encryption operation and persisted together with the ciphertext and authentication tag.

Management APIs are intentionally allowed to return decrypted credentials. If a management response includes `LoginToken` or `AgentSecret`, decrypt them before mapping the response.

Do not implement masked-secret responses, separate reveal endpoints, or one-time-only secret display unless explicitly requested later.

This is an intentional V1 product decision prioritizing operational usability.

Secrets must still never be written to logs, telemetry, or exception messages.

## 21. Error Handling

Keep error handling simple and explicit.

Do not create a large custom exception hierarchy unless required.

Distinguish between:

```text
Gateway / transport failures
and
Accounting operation failures
```

Examples of Gateway failures:

```text
ConnectorNotFound
AgentNotConnected
CommandTimeout
SignalRTransportFailure
```

Provider-specific failures originate from the Agent.

The Gateway must not implement provider-specific error interpretation.

HTTP responses must remain compatible with the documented External Accounting API contract.

## 22. Cancellation and Timeout

Asynchronous operations should accept and propagate `CancellationToken`, especially across:

```text
HTTP requests
database operations
command dispatch
SignalR command execution
```

Commands sent to an Agent must have a bounded lifetime.

Do not introduce automatic retries for write commands.

In particular, do not automatically retry:

```text
SaveContactCommand
SaveInvoiceCommand
DeleteInvoiceCommand
BatchSaveItemsCommand
```

after an ambiguous timeout.

V1 does not provide end-to-end idempotency.

## 23. Async Code

Use `async` / `await` for I/O operations.

Do not use:

```csharp
.Result
.Wait()
.GetAwaiter().GetResult()
```

in normal application flow.

Avoid unnecessary `Task.Run` around asynchronous I/O.

## 24. Dependency Injection

Use built-in ASP.NET Core dependency injection.

Prefer constructor injection.

Do not introduce an additional IoC container.

Register dependencies in clear extension methods when registration becomes large enough to justify it.

Do not create abstraction layers solely for dependency registration.

## 25. Database Access

Database persistence belongs to Infrastructure.

Application and Domain must not depend on Entity Framework or database-specific APIs.

Keep persistence straightforward.

Do not introduce generic repository abstractions unless there is a real need.

Prefer focused repositories such as `IConnectorRepository` when an abstraction is needed.

## 26. Implementation Workflow

For every task:

### Step 1 — Understand

Read the user request carefully.

Identify the smallest required change.

### Step 2 — Read Relevant Documents

Read only the architecture/contract documents relevant to the task.

### Step 3 — Inspect Existing Code

Inspect the files directly involved in the change.

Do not explore or redesign unrelated parts of the repository.

### Step 4 — Implement

Implement the smallest clean solution that satisfies the requirement.

### Step 5 — Verify

Verify:

```text
the solution builds
.NET 8 compatibility is preserved
architecture boundaries are preserved
public contracts were not unintentionally changed
no unrelated files were modified
```

Do not add tests unless explicitly requested.

## 27. When Requirements Are Unclear

Do not invent business behavior.

First check:

```text
EXTERNAL_ACCOUNTING_API_CONTRACT.md
GATEWAY_COMMAND_CONTRACT.md
CONNECTOR_AUTHENTICATION_DESIGN.md
existing implementation
```

If the answer is still not defined, preserve the smallest reasonable behavior required by the current task.

Do not introduce speculative features.

## 28. Definition of Done

A task is complete when:

```text
the requested behavior is implemented
the project still targets .NET 8
the solution builds
Clean Architecture boundaries are respected
SignalR boundaries are respected
relevant contracts are preserved
code is simple and readable
one top-level type exists per .cs file
no tests were added
no unnecessary abstractions were introduced
no unrelated files were changed
```

## 29. Primary Rule

When choosing between:

```text
a sophisticated implementation
and
a simple readable implementation
```

choose the simple readable implementation unless the sophisticated solution is required by the documented behavior.

The most important long-term requirement of this repository is:

> A future developer must be able to understand and safely modify the code with minimal effort.

Implement only what is needed now.
