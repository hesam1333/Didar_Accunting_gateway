# Gateway Command Contract

**Status:** Draft V1

## 1. Purpose

This document defines how the public External Accounting API implemented by the Gateway maps to internal commands executed by connected accounting agents.

The existing HTTP contract is defined in:

`EXTERNAL_ACCOUNTING_API_CONTRACT.md`

That document is the source of truth for HTTP routes, request DTOs, response DTOs, and compatibility requirements.

This document does **not** redefine the external API contract.

Its purpose is to define the internal boundary:

`HTTP API → Application Command → Connected Agent → Command Result → HTTP Response`

## 2. Core Principle

The Gateway must remain accounting-provider agnostic.

The Gateway knows:

- the External Accounting API contract
- which agent belongs to the current connection/account
- which command must be executed
- how to send that command to the agent
- how to correlate command results
- how to handle connection state and timeout

The Gateway must **not** know:

- Holoo database tables
- Holoo business rules
- Holoo SQL queries
- Holoo-specific identifiers
- how contacts, products, or invoices are stored internally in Holoo

All provider-specific behavior belongs to the local Agent.

## 3. Command Flow

For every supported endpoint, the flow is:

```text
HTTP Request
    ↓
Gateway API Endpoint
    ↓
Application Command
    ↓
IConnectorCommandDispatcher
    ↓
Connected Agent
    ↓
Provider-specific execution
    ↓
Command Result
    ↓
Gateway
    ↓
External Accounting API Response
```

The HTTP layer must remain thin.

It is responsible only for:

1. Receiving the External Accounting API request.
2. Resolving the appropriate connected agent.
3. Creating the corresponding Application Command.
4. Dispatching the command.
5. Converting the result to the expected HTTP response.

## 4. Command Dispatcher

The Application layer communicates with agents through an abstraction.

Conceptually:

```csharp
IConnectorCommandDispatcher
```

The Application layer must not depend directly on SignalR.

SignalR is an Infrastructure implementation detail.

Conceptual flow:

```text
Application
    ↓
IConnectorCommandDispatcher
    ↓
Infrastructure
    ↓
SignalR
    ↓
Windows Agent
```

The exact interface signature will be defined during implementation.

## 5. Common Command Metadata

Every command sent from the Gateway to an Agent must contain enough metadata to correlate the request and response.

At minimum:

```text
CommandId
CommandType
```

`CommandId` must uniquely identify a single command execution.

The same `CommandId` must be returned with the Agent result.

Conceptually:

```json
{
  "commandId": "guid",
  "commandType": "SearchContacts",
  "payload": {}
}
```

Result:

```json
{
  "commandId": "guid",
  "success": true,
  "result": {}
}
```

The exact transport DTO will be defined during implementation.

## 6. Endpoint → Command Mapping

### Setting

| External API Endpoint | Application Command |
|---|---|
| `POST /v1/setting/GetBusinessInfo` | `GetBusinessInfoCommand` |
| `POST /v1/setting/GetProjects` | `GetProjectsCommand` |

### Contacts

| External API Endpoint | Application Command |
|---|---|
| `POST /v1/contact/get` | `GetContactCommand` |
| `POST /v1/contact/getcontacts` | `SearchContactsCommand` |
| `POST /v1/contact/save` | `SaveContactCommand` |

### Invoices

| External API Endpoint | Application Command |
|---|---|
| `POST /v1/invoice/get` | `GetInvoiceCommand` |
| `POST /v1/invoice/save` | `SaveInvoiceCommand` |
| `POST /v1/invoice/delete` | `DeleteInvoiceCommand` |

### Items

| External API Endpoint | Application Command |
|---|---|
| `POST /v1/item/get` | `GetItemCommand` |
| `POST /v1/item/getitems` | `SearchItemsCommand` |
| `POST /v1/item/batchsave` | `BatchSaveItemsCommand` |

## 7. Command Definitions

### GetBusinessInfoCommand

Source endpoint: `POST /v1/setting/GetBusinessInfo`

Purpose: Ask the connected accounting Agent for information about the accounting business.

Input: No operation-specific input beyond the common accounting request context.

Output: Business information compatible with the response contract defined in `EXTERNAL_ACCOUNTING_API_CONTRACT.md`.

### GetProjectsCommand

Source endpoint: `POST /v1/setting/GetProjects`

Purpose: Return the projects available in the connected accounting system.

Input: No operation-specific input.

Output: Project list compatible with the External Accounting API contract.

### GetContactCommand

Source endpoint: `POST /v1/contact/get`

Purpose: Retrieve a single accounting contact by code.

Input: `Code`

Output: Contact data compatible with the External Accounting API contract.

### SearchContactsCommand

Source endpoint: `POST /v1/contact/getcontacts`

Purpose: Search or list contacts from the connected accounting system.

Input:

```text
QueryInfo
    SortBy
    SortDesc
    Take
    Skip
    Filters
```

Output: Paginated contact result compatible with the External Accounting API contract.

The Gateway must not interpret provider-specific filter behavior. The Agent is responsible for translating supported filters to the accounting provider.

### SaveContactCommand

Source endpoint: `POST /v1/contact/save`

Purpose: Create or update a contact in the connected accounting system according to the semantics of the accounting provider.

Input: `Contact`

Output: The saved contact compatible with the External Accounting API contract.

The Gateway must not decide how a contact is persisted or whether the operation represents create vs update. That behavior belongs to the Agent/provider implementation.

### GetInvoiceCommand

Source endpoint: `POST /v1/invoice/get`

Purpose: Retrieve an invoice from the connected accounting system.

Input:

```text
Number
Type
```

Output: Invoice data compatible with the External Accounting API contract.

### SaveInvoiceCommand

Source endpoint: `POST /v1/invoice/save`

Purpose: Create or update an invoice in the connected accounting system.

Input: `Invoice`

The command payload includes the complete invoice structure required by the External Accounting API contract, including:

- contact
- invoice items
- quantities
- prices
- discounts
- tax
- additional costs
- project
- currency
- other supported invoice fields

Output: The saved invoice compatible with the External Accounting API contract.

The Gateway must not automatically create missing contacts or items. Any accounting-specific persistence behavior belongs to the Agent.

### DeleteInvoiceCommand

Source endpoint: `POST /v1/invoice/delete`

Purpose: Delete an invoice from the connected accounting system.

Input:

```text
Number
Type
```

Output: `bool`, according to the existing External Accounting API contract.

### GetItemCommand

Source endpoint: `POST /v1/item/get`

Purpose: Retrieve a single accounting item/product by code.

Input: `Code`

Output: Item data compatible with the External Accounting API contract.

### SearchItemsCommand

Source endpoint: `POST /v1/item/getitems`

Purpose: Search or list products/items from the connected accounting system.

Input:

```text
QueryInfo
    SortBy
    SortDesc
    Take
    Skip
    Filters
```

Output: Paginated item result compatible with the External Accounting API contract.

The Gateway must not contain provider-specific search logic.

### BatchSaveItemsCommand

Source endpoint: `POST /v1/item/batchsave`

Purpose: Create or update multiple accounting items in one operation.

Input: `Items[]`

Output: List of saved items compatible with the External Accounting API contract.

Provider-specific batch behavior belongs to the Agent.

## 8. Request Mapping Rule

HTTP request DTOs and internal command DTOs may be separate types. However, the mapping should remain intentionally simple.

Example:

```text
GetContactsRequestDto
        ↓
SearchContactsCommand
```

The Application command should contain only the data required to perform the accounting operation.

Transport-specific information such as:

```text
BaseUrl
HTTP headers
ASP.NET HttpContext
SignalR connection IDs
```

must not become part of the business command payload unless explicitly required by the command execution model.

## 9. Response Mapping Rule

The Agent returns the result of the accounting operation.

The Gateway then wraps that result using the HTTP response contract defined in `EXTERNAL_ACCOUNTING_API_CONTRACT.md`.

The Agent should not need to know about ASP.NET response types or controller response wrappers.

Conceptually:

```text
Agent Result
    ↓
Application Result
    ↓
HTTP Contract Response
```

## 10. Agent Resolution

Before dispatching a command, the Gateway must determine which connected Agent should execute it.

Agent resolution must be based on the credentials / connector identity associated with the incoming accounting request.

The exact authentication and connector-registration model is defined separately.

HTTP endpoints must not directly inspect SignalR connections.

Agent resolution and command dispatch belong behind Application abstractions.

## 11. Connectivity

An accounting connection is usable only when the required local Agent is connected and capable of reaching its accounting system.

Connectivity therefore has at least two relevant states:

```text
AgentConnected
ProviderAvailable
```

A connected SignalR session alone does not guarantee that the local accounting database/provider is available.

The exact health-check protocol may be defined separately.

## 12. Command Timeout

Commands are synchronous from the perspective of the HTTP caller:

```text
HTTP Request
    ↓
Send Command
    ↓
Wait for Agent Result
    ↓
HTTP Response
```

Every dispatched command must therefore have a bounded execution lifetime.

The Gateway must be able to distinguish at least:

```text
AgentNotConnected
CommandTimeout
CommandFailed
CommandSucceeded
```

The exact timeout values and error codes will be defined during implementation.

## 13. Error Ownership

Errors should be separated into two categories.

### Gateway / Transport Errors

Examples:

```text
AgentNotConnected
CommandTimeout
InvalidConnector
SignalRTransportFailure
```

These originate in the Gateway infrastructure.

### Accounting Operation Errors

Examples:

```text
ContactNotFound
InvalidInvoice
ProviderValidationFailure
DatabaseFailure
```

These originate from the Agent/accounting provider.

The Gateway should transport accounting errors without embedding provider-specific business logic.

The HTTP layer must eventually map both categories to the compatibility response expected by `EXTERNAL_ACCOUNTING_API_CONTRACT.md`.

## 14. Retry and Idempotency

Automatic retries for write commands must not be introduced implicitly.

In particular:

```text
SaveContactCommand
SaveInvoiceCommand
DeleteInvoiceCommand
BatchSaveItemsCommand
```

may cause persistent side effects.

Until an explicit idempotency strategy exists, the Gateway must not automatically re-execute write commands after an ambiguous timeout.

Idempotency is outside the initial V1 scope and should be treated as known technical debt.

## 15. Architectural Rules

The following rules are mandatory:

1. The HTTP contract is defined by `EXTERNAL_ACCOUNTING_API_CONTRACT.md`.
2. Every external accounting endpoint maps to one explicit Application command.
3. Controllers/endpoints must remain thin.
4. Application must not depend on SignalR.
5. Application must not contain Holoo-specific logic.
6. Gateway Infrastructure may implement SignalR communication.
7. Accounting-provider-specific behavior belongs to the Agent.
8. Every command must have a unique correlation identifier.
9. Every command must support cancellation/timeout.
10. Write commands must not be automatically retried unless an explicit idempotency mechanism is introduced.
11. The Agent must not depend on ASP.NET HTTP response models.
12. HTTP compatibility concerns must remain at the Gateway boundary.

## 16. Current Command Set

The V1 Gateway currently requires these 11 commands:

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

This list is derived from the current External Accounting API contract.

Any new command added later must correspond either to:

- a new external accounting endpoint, or
- an explicitly documented internal Gateway capability.

Do not invent additional accounting commands without a concrete requirement.
