# Connector & Authentication Design

**Status:** Draft V1

---

## 1. Purpose

This document defines how the Gateway identifies accounting connections, authenticates incoming External Accounting API requests, authenticates connected Agents, stores credentials, and exposes Connector provisioning/management APIs.

The Gateway has **no management UI**. Connector creation, credential generation, retrieval, rotation, status management, and other administrative operations are performed through HTTP APIs.

The design must support multiple accounting providers while keeping the Gateway provider-agnostic.

The main identities are:

```text
BizDomainId
ConnectorId
ApiKey + LoginToken
ConnectorId + AgentSecret
```

---

# 2. Core Identity Model

Each configured accounting integration is represented by a single:

```text
Connector
```

A Connector represents the relationship between:

```text
Business
    +
Accounting Provider
    +
Local Agent
```

Example:

```text
BizDomainId:    B1
ConnectorId:    C1
Provider:       holoo
```

Another provider for another business could be:

```text
BizDomainId:    B2
ConnectorId:    C2
Provider:       mahak
```

The rest of the Gateway architecture must not depend on provider-specific implementation details.

---

# 3. Connector Entity

Initial V1 persistence model:

```text
Connector

Id
BizDomainId
Provider

ApiKey

LoginTokenEncrypted
AgentSecretEncrypted

IsActive

CreatedAt
UpdatedAt

LastAgentConnectedAt
LastAgentSeenAt
```

Optional operational fields may be added later.

---

# 4. Credential Roles

Two independent credential sets exist.

## External API Credential

Used by callers of the External Accounting API:

```text
ApiKey
LoginToken
```

Purpose:

```text
Caller
    ↓
Gateway HTTP API
```

These credentials identify and authenticate the Connector associated with an incoming accounting request.

---

## Agent Credential

Used by the local accounting Agent:

```text
ConnectorId
AgentSecret
```

Purpose:

```text
Local Agent
    ↓
Gateway Authentication
    ↓
SignalR
```

The Agent MUST NOT use the External API `LoginToken` for authentication.

External API authentication and Agent authentication are separate trust boundaries.

---

# 5. ApiKey

`ApiKey` is the public identifier of a Connector.

Example:

```text
ak_G42F8x...
```

It is used to efficiently locate the Connector:

```text
ApiKey
    ↓
Connector
```

`ApiKey` is not considered a secret and may be stored in plaintext.

A unique database index should exist on:

```text
ApiKey
```

---

# 6. LoginToken

`LoginToken` is the secret used to authenticate incoming External Accounting API requests.

Example:

```text
lt_E4qr7Yx...
```

Incoming requests contain:

```text
ApiKey
LoginToken
BizDomainId
```

Authentication flow:

```text
ApiKey
    ↓
Find Connector
    ↓
Decrypt stored LoginToken
    ↓
Compare LoginToken
    ↓
Validate BizDomainId
    ↓
Validate Provider
    ↓
Request Authenticated
```

The `LoginToken` must be cryptographically random and sufficiently long.

It is not an OAuth access token.

It does not require periodic refresh.

It remains valid until explicitly rotated or the Connector is disabled.

---

# 7. Why Tokens Are Encrypted Instead of Hashed

Management API consumers must be able to retrieve the current credentials again.

Because of this requirement, one-way password hashing is not suitable for these credentials.

A hash cannot be decoded.

Therefore:

```text
LoginToken
AgentSecret
```

must use reversible authenticated encryption.

They MUST NOT be stored as plaintext.

---

# 8. Encryption at Rest

V1 uses authenticated symmetric encryption for credential storage.

Required V1 algorithm:

```text
AES-256-GCM
```

Conceptually:

```text
LoginToken
    ↓
AES-GCM Encrypt
    ↓
Encrypted value stored in DB
```

When required:

```text
Encrypted value
    ↓
AES-GCM Decrypt
    ↓
LoginToken
```

The same model applies to:

```text
AgentSecret
```

---

# 9. Encryption Key and Configuration

The AES master key is stored in application configuration.

For V1, the encryption configuration is loaded from:

```text
appsettings.json
appsettings.{Environment}.json
```

or configuration providers overriding the same settings at deployment time.

Example conceptual configuration:

```json
{
  "Encryption": {
    "Key": "<base64-encoded-256-bit-key>"
  }
}
```

The key must contain strong random 256-bit key material.

The application must not generate a new encryption key automatically at startup. Changing the configured key without re-encrypting existing credentials will make existing secrets impossible to decrypt.

AES-GCM does not require a password salt when a strong random key is used directly. Any static encryption configuration material belongs in `appsettings`; the per-encryption nonce is generated randomly and stored with the encrypted value.

---

# 10. Encrypted Credential Format

Every encrypted credential must contain all data required for AES-256-GCM decryption:

```text
CipherText
Nonce
AuthenticationTag
```

A fresh cryptographically secure nonce MUST be generated for every encryption operation performed with the same master key.

The nonce is not secret and is persisted with the ciphertext and authentication tag.

Never reuse a nonce with the same AES-GCM key.

The values may be stored as separate columns or encoded together in a versioned value such as:

```text
v1:<nonce>:<ciphertext>:<tag>
```

The exact persistence representation is an implementation detail.

---

# 11. Credential Generation

When a Connector is created, the Gateway generates:

```text
ConnectorId
ApiKey
LoginToken
AgentSecret
```

Tokens MUST be generated using a cryptographically secure random number generator.

They must not be generated from:

```text
BizDomainId
UserId
Timestamp
Provider
Sequential IDs
```

or any other predictable information.

---

# 12. Connector Provisioning API

The Gateway does not provide a provisioning page or management UI.

It exposes an HTTP API for creating Connector records.

The create operation receives at least:

```text
BizDomainId
Provider
```

The Gateway then:

```text
creates ConnectorId
generates ApiKey
generates LoginToken
generates AgentSecret
encrypts LoginToken
encrypts AgentSecret
persists the Connector
returns the Connector credentials
```

A management response may include:

```text
ConnectorId
BizDomainId
Provider
BaseUrl
ApiKey
LoginToken
AgentSecret
IsActive
```

`LoginToken` and `AgentSecret` are decrypted before being returned.

---

# 13. Retrieving Existing Credentials

The Gateway exposes management APIs rather than a UI.

There is no masked-secret model in V1.

When an authorized management API returns a Connector and its credentials, the Gateway must return:

```text
ApiKey
LoginToken
AgentSecret
```

with `LoginToken` and `AgentSecret` decrypted.

Flow:

```text
Load Connector
    ↓
Decrypt LoginToken
    ↓
Decrypt AgentSecret
    ↓
Map management response
    ↓
Return response
```

Do not require separate operations such as:

```text
RevealLoginToken
RevealAgentSecret
ShowSecret
```

Credential retrieval is part of the normal management API response whenever that response includes credentials.

This is an intentional V1 usability decision. Decrypted credentials must still never be persisted as plaintext or written to logs, telemetry, or exception messages.

---

# 14. Credential Rotation

Both secrets can be independently rotated.

Available operations:

```text
Rotate LoginToken

Rotate AgentSecret
```

Rotation means:

```text
Generate new random secret
    ↓
Encrypt new secret
    ↓
Replace stored encrypted value
    ↓
Old secret becomes invalid
Return new decrypted secret in the management API response
```

`Refresh` should not be used as terminology because these credentials are not OAuth tokens.

---

# 15. LoginToken Rotation

Rotating the LoginToken affects:

```text
External API Caller → Gateway
```

After rotation, the configured accounting integration must use the new token.

The Agent is not affected.

---

# 16. AgentSecret Rotation

Rotating the AgentSecret affects:

```text
Agent → Gateway
```

After rotation, the Agent configuration must be updated.

Existing and future Agent authentication using the previous secret must fail.

The External Accounting API caller is not affected.

---

# 17. Provider Routing

Each provider receives a separate public BaseUrl.

Example:

```text
https://gateway.example.com/holoo

https://gateway.example.com/mahak

https://gateway.example.com/sepidar
```

The existing accounting service adds its `/v1/...` routes to this BaseUrl.

Therefore requests become:

```text
POST /holoo/v1/contact/get

POST /mahak/v1/contact/get

POST /sepidar/v1/contact/get
```

The provider segment is used as part of Connector validation.

Conceptually:

```text
Route Provider
      +
ApiKey
      ↓
Connector
```

The Gateway MUST verify:

```text
RouteProvider == Connector.Provider
```

---

# 18. ProviderType in Request Body

The legacy External Accounting API request contains:

```text
ProviderType
```

The Gateway must preserve compatibility with this field where required by the external contract.

However, `ProviderType` from the request body MUST NOT be considered the authoritative source for Gateway routing.

Gateway routing uses:

```text
Route Provider
+
Connector configuration
```

as the source of truth.

---

# 19. BizDomain Validation

After resolving the Connector through `ApiKey`, the Gateway validates:

```text
Request.BizDomainId == Connector.BizDomainId
```

A mismatch must reject the request.

`BizDomainId` should not be used as the primary credential or Connector lookup key.

It represents ownership/tenant context, not authentication.

---

# 20. External API Authentication Flow

Complete incoming HTTP flow:

```text
POST /{provider}/v1/...

Request:
    ApiKey
    LoginToken
    BizDomainId

        ↓

Resolve Connector using ApiKey

        ↓

Connector exists?

        ↓

Connector.IsActive?

        ↓

Route Provider == Connector.Provider?

        ↓

BizDomainId == Connector.BizDomainId?

        ↓

Decrypt stored LoginToken

        ↓

Constant-time comparison

        ↓

Authenticated ConnectorId

        ↓

Create Application Command

        ↓

Dispatch to Agent
```

Authentication concerns should be implemented outside individual accounting endpoint handlers wherever practical.

Endpoint handlers must not duplicate credential-validation logic.

---

# 21. Agent Authentication

The Agent uses:

```text
ConnectorId
AgentSecret
```

to authenticate with the Gateway.

The Agent does not connect directly to SignalR using the External API credentials.

Recommended flow:

```text
Agent
   ↓
Agent Authentication Endpoint
   ↓
ConnectorId + AgentSecret
   ↓
Validate Connector
   ↓
Decrypt AgentSecret
   ↓
Validate Secret
   ↓
Issue short-lived access token
   ↓
SignalR connection
```

---

# 22. Agent Access Token

After successful Agent authentication, the Gateway issues a short-lived signed access token.

JWT is suitable for V1.

The token may contain claims such as:

```text
ConnectorId
BizDomainId
Provider
```

Example lifetime:

```text
30–60 minutes
```

The exact lifetime is an implementation configuration.

The Agent is responsible for obtaining a new token before or after expiration.

`AgentSecret` remains the long-lived credential.

The JWT is the short-lived connection credential.

---

# 23. SignalR Hub

All accounting Agents connect to the same logical SignalR Hub.

Example:

```text
/hubs/accounting-agent
```

The Gateway SHOULD NOT create provider-specific hubs such as:

```text
/hubs/holoo
/hubs/mahak
/hubs/sepidar
```

The SignalR transport is provider-agnostic.

---

# 24. SignalR Identity

After authentication, every SignalR connection must be associated with a:

```text
ConnectorId
```

Conceptual runtime registry:

```text
ConnectorId → SignalR Connection
```

Example:

```text
Connector C1 → Connection A
Connector C2 → Connection B
Connector C3 → Connection C
```

The command dispatcher should work with `ConnectorId`, not provider-specific connection concepts.

---

# 25. Command Dispatch

Once the HTTP request has been authenticated:

```text
ConnectorId
    ↓
IConnectorCommandDispatcher
    ↓
Resolve active Agent connection
    ↓
Send Command
```

Provider-specific behavior starts only after the command reaches the local Agent.

The Gateway command transport remains identical regardless of provider.

---

# 26. Agent Configuration

The local Agent requires only Gateway-related identity information.

Initial configuration:

```text
GatewayUrl
ConnectorId
AgentSecret
```

Provider-specific configuration such as:

```text
Database connection
Accounting software settings
Local credentials
```

belongs to the Agent itself and must not become part of Gateway authentication.

---

# 27. Connector Status

Management APIs may expose at least:

```text
Provider
BizDomainId

ApiKey

Agent Status
Provider Status

CreatedAt
LastAgentSeenAt
```

Possible status representation:

```text
Agent Status:
Online / Offline

Provider Status:
Healthy / Unavailable / Unknown
```

Agent connectivity and accounting-provider availability are different concepts.

---

# 28. Disable Connector

An administrator can disable a Connector:

```text
IsActive = false
```

A disabled Connector must reject:

```text
External API authentication
Agent authentication
Command execution
```

Existing Agent connections should also be terminated or prevented from executing new commands.

---

# 29. Security and Usability Rules

The following rules are mandatory for V1:

1. `ApiKey` may be stored in plaintext.

2. `LoginToken` MUST NOT be stored in plaintext.

3. `AgentSecret` MUST NOT be stored in plaintext.

4. `LoginToken` and `AgentSecret` MUST NOT be hashed; they must remain decryptable.

5. Use AES-256-GCM for reversible authenticated encryption.

6. The AES master key is loaded from `appsettings` / application configuration.

7. Generate a unique cryptographically secure nonce for every encryption operation and store it with the ciphertext and authentication tag.

8. External API credentials and Agent credentials must remain independent.

9. Authorized management API responses may return decrypted `LoginToken` and `AgentSecret`.

10. Do not require masked-secret responses or dedicated reveal-secret endpoints in V1.

11. Secrets must never be written to application logs, telemetry, or exception messages.

12. Request/response logging must redact secret fields.

13. Secret comparison should use constant-time comparison where practical.

14. Disabled Connectors must not authenticate or execute commands.

15. Route provider, Connector provider, and BizDomain ownership must be validated before command execution.

V1 deliberately prioritizes operational usability for authorized management consumers while still protecting secrets at rest in the database.

---

# 30. V1 Responsibility Summary

The authentication architecture is:

```text
                        Gateway Database
                              │
                    ┌─────────┴─────────┐
                    │                   │
              Connector Metadata   Encrypted Secrets
                    │
                    │
        ┌───────────┴────────────┐
        │                        │
        ▼                        ▼
External API                 Local Agent

ApiKey                       ConnectorId
LoginToken                   AgentSecret
        │                        │
        ▼                        ▼
HTTP Authentication         Agent Authentication
                                 │
                                 ▼
                           Short-lived JWT
                                 │
                                 ▼
                              SignalR
```

The Gateway resolves both sides to the same:

```text
ConnectorId
```

After authentication, the rest of the command architecture works only with the Connector identity.

---

# 31. V1 Design Decisions

For V1:

- Use one `Connector` record per accounting integration.
- Use `ApiKey + LoginToken` for External Accounting API authentication.
- Use `ConnectorId + AgentSecret` for Agent authentication.
- Use AES-256-GCM for reversible authenticated encryption.
- Never hash `LoginToken` or `AgentSecret`.
- Never persist `LoginToken` or `AgentSecret` as plaintext.
- Keep the AES master key in `appsettings` / application configuration.
- Generate a unique random nonce for every encryption operation.
- Persist nonce and authentication tag with ciphertext.
- Build no Gateway management UI.
- Expose Connector provisioning and management through HTTP APIs only.
- Return decrypted credentials from authorized management API responses whenever those responses include credentials.
- Do not require separate reveal-secret endpoints.
- Allow independent rotation of `LoginToken` and `AgentSecret`.
- Use provider name in the public API BaseUrl.
- Use one provider-agnostic SignalR Hub.
- Resolve SignalR connections by `ConnectorId`.
- Use a short-lived access token for authenticated Agent connections.
- Keep accounting-provider-specific implementation entirely inside the Agent.
