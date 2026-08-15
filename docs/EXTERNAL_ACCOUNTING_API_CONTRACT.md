# EXTERNAL ACCOUNTING API CONTRACT

**Scope:** The `HesabfaController` pipeline of the Didar Invoice API (`src/DidarInvoiceApi/Controllers/v1/HesabfaController.cs`) and the services/DTOs it directly uses.

**Audience:** An external accounting provider (or a gateway implementing the provider side) that wants the Didar Accounting Service to communicate with it.

> If you want the Accounting Service to communicate with your system, these are the endpoints, requests, responses, authentication rules, and behaviors your API must implement.

This document only describes what can be confirmed from the code. Anything that cannot be confirmed from the code is marked **`Unknown`**.

---

## 1. High-level data flow

```
Didar client
   │  HTTP POST  /api/v1/Hesabfa/{ActionName}      (header: BaseUrl = <provider base URL>)
   │  body: { apiKey, loginToken, providerType, userId, bizDomainId, ...action fields }
   ▼
Accounting Service (HesabfaController)
   │  builds address = <BaseUrl header> + "/v1/..."     (see mapping table below)
   │  HTTP POST  <BaseUrl>/v1/...                        (no auth headers; creds are in the body)
   │  body: same DTO, serialized with camelCase JSON
   ▼
External accounting provider API   ◄── THIS DOCUMENT DESCRIBES THIS SIDE
   │  must respond HTTP 200 with { success, errorCode, errorMessage, result }
   ▼
Accounting Service unwraps `result` and re-wraps it for the Didar client
```

- The provider API is reached with **only** the body as input. No query string, no route parameters, no additional HTTP headers are sent by the service code.
- The provider's HTTP responses are read as raw text and deserialized by the service with **Newtonsoft.Json (case-insensitive property matching)**.

---

## 2. Connection and authentication

| Item | Value / behavior | Confirmed source |
|---|---|---|
| Provider base URL | Provided by the Didar client as an HTTP header named **`BaseUrl`** on every request to the Accounting Service. The service reads `HttpContext.Request.Headers["BaseUrl"]` and builds every provider URL as `<BaseUrl> + "/v1/..."`. | `Hesabfa/Services/SettingService.cs:36`, `ContactService.cs:36`, `HesabfaInvoiceService.cs:44`, `Request.cs:32` |
| `ApiKey` | Sent **inside the JSON request body** (field `apiKey`). Not a header. | `Hesabfa/Dtos/HesabfaRequestBaseDto.cs:8` |
| `LoginToken` | Sent **inside the JSON request body** (field `loginToken`). Not a header. | `Hesabfa/Dtos/HesabfaRequestBaseDto.cs:9` |
| `ApiSecret` | **Not used anywhere in this flow.** No such field, header, or configuration exists in the code. | repo-wide search |
| `ProviderType` | Sent inside the body as `providerType`. Default value is `3` (enum `Hesabfa`). | `Hesabfa/Dtos/HesabfaRequestBaseDto.cs:10` |
| `userId`, `bizDomainId` | Didar-internal GUIDs forwarded inside the body. The provider may ignore them. | `src/Domain/Common/BaseDidarIdentity.cs` |
| Authentication model | The provider must authenticate using `apiKey` + `loginToken` received in the body. The Accounting Service performs no authentication itself. | — |
| Outbound HTTP headers | None added by code (the `headers` parameter is never passed). RestSharp only sets Content-Type `application/json` for the JSON body. | `Request.cs:39-73`, all service call sites |
| Timeout | `RestClientOptions.MaxTimeout = -1` is set in code. Effective timeout behavior is **`Unknown`** (value depends on the installed RestSharp 112.x semantics). | `Request.cs:48-51` |

**Note:** If the `BaseUrl` header is missing or empty, the provider URL becomes a relative path (`/v1/...`); the behavior then depends on the HTTP client. The header is treated as required in practice.

---

## 3. Common request body (sent to the provider)

Every provider endpoint receives a JSON body with at least these fields (camelCase serialization via System.Text.Json):

```json
{
  "apiKey": "string",
  "loginToken": "string",
  "providerType": 3,
  "userId": "00000000-0000-0000-0000-000000000000",
  "bizDomainId": "00000000-0000-0000-0000-000000000000"
}
```

Type mapping of these common fields:

| JSON field | Type | Notes |
|---|---|---|
| `apiKey` | string | non-nullable in DTO |
| `loginToken` | string | non-nullable in DTO |
| `providerType` | int (enum) | default `3` |
| `userId` | Guid (string in JSON) | default `00000000-0000-...` if not provided |
| `bizDomainId` | Guid (string in JSON) | default `00000000-0000-...` if not provided |

Serialization details of the outbound request:

- Serializer: `System.Text.Json` configured with `PropertyNamingPolicy = CamelCase` (`Request.cs:52-58`).
- Dates (`DateTime`) serialize as ISO 8601 strings (System.Text.Json default).
- Decimals serialize as JSON numbers.
- Properties without values keep their C# initializer defaults (e.g. empty strings, `0`, `false`, empty lists) because the full DTO object is serialized verbatim.

---

## 4. Common response envelope the provider must return

For **all** endpoints the Accounting Service deserializes the provider response into this shape (camelCase or PascalCase both work — Newtonsoft matches case-insensitively):

```json
{
  "success": true,
  "errorCode": 0,
  "errorMessage": null,
  "result": { }        // type depends on endpoint; see per-endpoint sections
}
```

Field semantics (from `Hesabfa/Dtos/HesabfaResponseBaseDto.cs`):

| JSON field | Type | Required for success |
|---|---|---|
| `success` | bool | `true` |
| `errorCode` | int | `0` (or any value; ignored when `success=true`) |
| `errorMessage` | string \| null | ignored when `success=true` |
| `result` | object (endpoint-specific) | must contain the payload when `success=true` |

`result` must be omitted or null when `success=false`.

---

## 5. Endpoint map (Accounting Service action → provider endpoint)

The controller action names map 1:1 to provider paths. All calls are `HTTP POST` with a JSON body (no query string, no route parameters).

| Controller action (client-facing) | Provider endpoint called by the service |
|---|---|
| `GetBusinessInfo` | `POST {BaseUrl}/v1/setting/GetBusinessInfo` |
| `GetProjects` | `POST {BaseUrl}/v1/setting/GetProjects` |
| `GetContactByCode` | `POST {BaseUrl}/v1/contact/get` |
| `GetContacts` | `POST {BaseUrl}/v1/contact/getcontacts` |
| `SaveContact` | `POST {BaseUrl}/v1/contact/save` |
| `GetInvoice` | `POST {BaseUrl}/v1/invoice/get` |
| `SaveInvoice` | `POST {BaseUrl}/v1/invoice/save` |
| `GetInvoiceItem` | `POST {BaseUrl}/v1/item/get` |
| `GetInvoiceItems` | `POST {BaseUrl}/v1/item/getitems` |
| `DeleteInvoice` | `POST {BaseUrl}/v1/invoice/delete` |
| `BatchSaveItem` | `POST {BaseUrl}/v1/item/batchsave` |

No request/response transformation happens in the pipeline: the Accounting Service forwards the incoming request DTO **verbatim** to the provider, and forwards the provider's `result` back to the client.

---

## 6. Per-endpoint contract

The notation below documents the **request body** (in addition to the common fields in §3) and the **`result` payload** (inside the §4 envelope) that the provider must accept / return.

### 6.1 `POST {BaseUrl}/v1/setting/GetBusinessInfo`

Request: common fields only (no additional body fields).

`result` — business info object:

```json
{
  "name": "string",
  "legalName": "string",
  "calendar": "string",
  "currency": "string",
  "supportMultiCurrency": true,
  "otherCurrencies": "string",
  "subscription": 0,
  "credit": 0,
  "expireDate": "2026-08-13T00:00:00"
}
```

| Field | Type |
|---|---|
| `name` | string |
| `legalName` | string |
| `calendar` | string |
| `currency` | string |
| `supportMultiCurrency` | bool |
| `otherCurrencies` | string |
| `subscription` | int |
| `credit` | int |
| `expireDate` | DateTime (ISO 8601 string in JSON) |

### 6.2 `POST {BaseUrl}/v1/setting/GetProjects`

Request: common fields only.

`result` — array of projects:

```json
[
  { "id": 1, "title": "string", "active": true }
]
```

| Field | Type |
|---|---|
| `id` | int |
| `title` | string |
| `active` | bool |

### 6.3 `POST {BaseUrl}/v1/contact/get`

Request body (common + `code`):

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "code": "string"
}
```

| Field | Type | Notes |
|---|---|---|
| `code` | string | default `""` |

`result` — contact object (see §6.5 for the full contact shape; `GetContactResponseDto`).

### 6.4 `POST {BaseUrl}/v1/contact/getcontacts`

Request body (common + `queryInfo`):

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "queryInfo": {
    "sortBy": "string",
    "sortDesc": false,
    "take": 0,
    "skip": 0,
    "filters": [ { "property": "string", "operator": "string", "value": "string" } ]
  }
}
```

| Field | Type | Notes |
|---|---|---|
| `queryInfo` | object | always present (initialized to a new instance) |
| `queryInfo.sortBy` | string | default `""` |
| `queryInfo.sortDesc` | bool | default `false` |
| `queryInfo.take` | int | default `0` |
| `queryInfo.skip` | int | default `0` |
| `queryInfo.filters` | array | default `[]` |
| `filters[].property` | string | |
| `filters[].operator` | string | |
| `filters[].value` | string | |

`result` — pagination envelope (same shape for contacts and items):

```json
{
  "totalCount": 0,
  "filteredCount": 0,
  "from": 0,
  "to": 0,
  "list": [ /* GetContactResponseDto */ ]
}
```

| Field | Type |
|---|---|
| `totalCount` | int |
| `filteredCount` | int |
| `from` | int |
| `to` | int |
| `list` | array of contact objects |

### 6.5 `POST {BaseUrl}/v1/contact/save`

Request body (common + `contact`):

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "contact": {
    "code": "string", "name": "string", "company": "string",
    "firstName": "string", "lastName": "string",
    "contactType": 0, "nodeFamily": "string",
    "nationalCode": null, "economicCode": null, "registrationNumber": null,
    "email": null, "address": null, "country": null, "city": null, "state": null,
    "postalCode": null, "phone": null, "fax": null, "mobile": null
  }
}
```

| Field | Type | Notes |
|---|---|---|
| `contact` | object \| null | nullable — may be `null` |
| `contact.code` | string | default `""` |
| `contact.name` | string | default `""` |
| `contact.company` | string | default `""` |
| `contact.firstName` | string | default `""` |
| `contact.lastName` | string | default `""` |
| `contact.contactType` | int | default `0` |
| `contact.nodeFamily` | string | default `""` |
| `contact.nationalCode` | string \| null | |
| `contact.economicCode` | string \| null | |
| `contact.registrationNumber` | string \| null | |
| `contact.email` | string \| null | |
| `contact.address` | string \| null | |
| `contact.country` | string \| null | |
| `contact.city` | string \| null | |
| `contact.state` | string \| null | |
| `contact.postalCode` | string \| null | |
| `contact.phone` | string \| null | |
| `contact.fax` | string \| null | |
| `contact.mobile` | string \| null | |

`result` — the saved contact, shape `GetContactResponseDto`:

```json
{
  "id": 0, "code": "string", "name": "string", "company": "string",
  "contactType": 0, "nationalCode": null, "economicCode": null, "registrationNumber": null,
  "firstName": "string", "lastName": "string", "email": null, "address": null,
  "country": null, "city": null, "state": null, "postalCode": null,
  "phone": null, "fax": null, "mobile": null,
  "contactEmail": null, "website": null, "registrationDate": null, "note": null,
  "isShareHolder": false, "isCustomer": false, "isVendor": false,
  "sharePercent": 0.0, "liability": 0.0, "credits": 0.0, "contactCredit": 0.0,
  "hesabfaKey": 0, "nodeId": 0, "nodeName": null, "nodeFamily": null, "tag": null,
  "active": false, "taxType": 0
}
```

| Field | Type |
|---|---|
| `id` | int |
| `code`, `name`, `company`, `firstName`, `lastName` | string |
| `contactType` | int |
| `nationalCode`, `economicCode`, `registrationNumber`, `email`, `address`, `country`, `city`, `state`, `postalCode`, `phone`, `fax`, `mobile`, `contactEmail`, `website`, `registrationDate`, `note`, `nodeName`, `nodeFamily`, `tag` | string \| null |
| `isShareHolder`, `isCustomer`, `isVendor`, `active` | bool |
| `sharePercent`, `liability`, `credits`, `contactCredit` | decimal (JSON number) |
| `hesabfaKey`, `nodeId`, `taxType` | int |

### 6.6 `POST {BaseUrl}/v1/invoice/get`

Request body (common + `number`, `type`):

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "number": 0, "type": 0
}
```

| Field | Type | Notes |
|---|---|---|
| `number` | int | default `0` |
| `type` | int | default `0` |

`result` — invoice object, shape `InvoiceResponseDto` (see §6.7).

### 6.7 `POST {BaseUrl}/v1/invoice/save`

Request body (common + `invoice`):

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "invoice": {
    "number": null, "reference": null, "date": "2026-08-13T00:00:00", "dueDate": "2026-08-13T00:00:00",
    "contactCode": "string", "contactTitle": "string", "note": null,
    "invoiceType": 0, "status": 0, "tag": null, "sent": null, "freight": null,
    "warehouseReceiptStatus": 0, "project": "string", "salesmanCode": "string", "salesmanPercent": null,
    "invoiceItems": [ { "rowNumber": null, "description": "string", "itemCode": null, "unit": "string",
                        "quantity": 0.0, "unitPrice": 0.0, "discount": 0.0, "tax": 0.0 } ],
    "others": [ { "title": "string", "amount": 0.0, "add": true } ],
    "currency": "string", "currencyRate": 0.0
  }
}
```

| Field | Type | Notes |
|---|---|---|
| `invoice` | object | non-nullable; must be provided |
| `invoice.number` | string \| null | `null` means "create" per code comment |
| `invoice.reference` | string \| null | |
| `invoice.date` | DateTime | ISO 8601 string; required |
| `invoice.dueDate` | DateTime | ISO 8601 string; required |
| `invoice.contactCode` | string | default `""` |
| `invoice.contactTitle` | string | default `""` |
| `invoice.note` | string \| null | |
| `invoice.invoiceType` | int | comment: usually `0` |
| `invoice.status` | int | comment: `0` = draft, `1` = invoice |
| `invoice.tag` | string \| null | |
| `invoice.sent` | bool \| null | |
| `invoice.freight` | decimal \| null | |
| `invoice.warehouseReceiptStatus` | int | default `0` |
| `invoice.project` | string | default `""` |
| `invoice.salesmanCode` | string | default `""` |
| `invoice.salesmanPercent` | double \| null | |
| `invoice.invoiceItems` | array | default `[]` |
| `invoice.invoiceItems[].rowNumber` | int \| null | |
| `invoice.invoiceItems[].description` | string | default `""` |
| `invoice.invoiceItems[].itemCode` | string \| null | |
| `invoice.invoiceItems[].unit` | string | default `""` |
| `invoice.invoiceItems[].quantity` | decimal | |
| `invoice.invoiceItems[].unitPrice` | decimal | |
| `invoice.invoiceItems[].discount` | decimal | |
| `invoice.invoiceItems[].tax` | decimal | |
| `invoice.others` | array | default `[]` |
| `invoice.others[].title` | string | default `""` |
| `invoice.others[].amount` | decimal | |
| `invoice.others[].add` | bool | |
| `invoice.currency` | string | default `""` |
| `invoice.currencyRate` | decimal | default `0` |

`result` — invoice object, shape `InvoiceResponseDto`:

```json
{
  "id": 0, "number": "string", "date": "2026-08-13T00:00:00", "dueDate": "2026-08-13T00:00:00",
  "contactCode": "string", "contact": { /* GetContactResponseDto */ }, "contactTitle": "string",
  "sum": 0.0, "payable": 0.0, "paid": 0.0, "rest": 0.0, "freight": 0.0,
  "freightPersonCode": "string", "freightPersonName": "string", "note": "string",
  "sent": false, "returned": false, "invoiceType": 0, "status": 0,
  "invoiceItems": [ { "id": 0, "rowNumber": 0, "description": "string", "itemCode": "string",
                      "item": { /* ItemDto */ }, "quantity": 0.0, "unit": "string",
                      "unitPrice": 0.0, "sum": 0.0, "discount": 0.0, "tax": 0.0,
                      "totalAmount": 0.0, "serialNumbers": [] } ],
  "tag": "string", "warehouseReceiptStatus": 0, "project": "string",
  "salesmanCode": "string", "salesmanPercent": 0.0,
  "others": [ { "title": "string", "amount": 0.0, "add": true } ],
  "currency": "string", "currencyRate": 0.0, "taxId": "string"
}
```

`InvoiceResponseDto` fields:

| Field | Type |
|---|---|
| `id` | int |
| `number` | string |
| `date`, `dueDate` | DateTime (ISO 8601) |
| `contactCode` | string |
| `contact` | contact object (`GetContactResponseDto`) — always present (initialized) |
| `contactTitle`, `freightPersonCode`, `freightPersonName`, `note`, `tag`, `project`, `salesmanCode`, `currency`, `taxId` | string |
| `sum`, `payable`, `paid`, `rest`, `freight` | decimal |
| `sent`, `returned` | bool |
| `invoiceType`, `status`, `warehouseReceiptStatus` | int |
| `salesmanPercent` | double |
| `invoiceItems` | array of line items |
| `others` | array of other-costs |

`InvoiceItemDto` (line item) fields:

| Field | Type |
|---|---|
| `id`, `rowNumber` | int |
| `description`, `itemCode`, `unit` | string |
| `item` | embedded item object (`ItemDto`) — always present (initialized) |
| `quantity`, `unitPrice`, `sum`, `discount`, `tax`, `totalAmount` | decimal |
| `serialNumbers` | array of string — default `[]` |

`ItemDto` (embedded item) fields: `id` (int), `code`, `name`, `unit`, `subUnit`, `purchasesTitle`, `salesTitle`, `nodeName`, `nodeFamily`, `tag`, `description`, `productCode`, `taxId`, `taxUnit` (string), `itemType`, `nodeId` (int), `conversionFactor` (decimal), `barcode` (string), `stock` (decimal), `buyPrice`, `sellPrice` (decimal), `active` (bool), `priceList` (object \| null — shape not documented).

`OtherCostDto` fields: `title` (string), `amount` (decimal), `add` (bool).

### 6.8 `POST {BaseUrl}/v1/item/get`

Request body (common + `code`):

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "code": "string"
}
```

`result` — item object, shape `ItemResponseDto`:

```json
{
  "id": 0, "code": "string", "name": "string", "itemType": 0, "unit": "string",
  "subUnit": "string", "conversionFactor": 0.0, "barcode": "string", "stock": 0.0,
  "buyPrice": 0.0, "sellPrice": 0.0, "purchasesTitle": "string", "salesTitle": "string",
  "nodeId": 0, "nodeName": "string", "nodeFamily": "string", "tag": "string",
  "description": "string", "productCode": "string", "active": false,
  "taxId": "string", "taxUnit": "string"
}
```

| Field | Type |
|---|---|
| `id`, `itemType`, `nodeId` | int |
| `code`, `name`, `unit`, `subUnit`, `barcode`, `purchasesTitle`, `salesTitle`, `nodeName`, `nodeFamily`, `tag`, `description`, `productCode`, `taxId`, `taxUnit` | string |
| `conversionFactor`, `stock` | double |
| `buyPrice`, `sellPrice` | decimal |
| `active` | bool |

### 6.9 `POST {BaseUrl}/v1/item/getitems`

Request body: common + `queryInfo` (same structure as §6.4).

`result` — pagination envelope (same structure as §6.4) whose `list` is an array of `ItemResponseDto` (shape in §6.8).

### 6.10 `POST {BaseUrl}/v1/invoice/delete`

Request body (common + `number`, `type`) — identical to §6.6:

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "number": 0, "type": 0
}
```

`result` — boolean:

```json
true
```

### 6.11 `POST {BaseUrl}/v1/item/batchsave`

Request body (common + `items`):

```json
{
  "apiKey": "...", "loginToken": "...", "providerType": 3, "userId": "...", "bizDomainId": "...",
  "items": [
    { "code": null, "name": "string", "itemType": 0, "unit": "string", "sellPrice": 0.0,
      "salesTitle": "string", "description": "string", "productCode": "string",
      "active": false, "nodeFamily": "string" }
  ]
}
```

| Field | Type | Notes |
|---|---|---|
| `items` | array | default `[]` |
| `items[].code` | string \| null | |
| `items[].name` | string | default `""` |
| `items[].itemType` | int | default `0` |
| `items[].unit` | string | default `""` |
| `items[].sellPrice` | decimal | default `0` |
| `items[].salesTitle` | string | default `""` |
| `items[].description` | string | default `""` |
| `items[].productCode` | string | default `""` |
| `items[].active` | bool | default `false` |
| `items[].nodeFamily` | string | default `""` |

`result` — array of `ItemResponseDto` (shape in §6.8).

---

## 7. Error behavior the Accounting Service enforces

How the service reacts to the provider's HTTP response (identical logic in all three services):

| Provider behavior | Accounting Service behavior | What the Didar client ultimately sees |
|---|---|---|
| HTTP status code indicates failure (`response.IsSuccessful == false` — any 4xx/5xx or transport error) | Service returns `null` from the action. | HTTP 200 with an **empty body**. A `RequestLog` is written to the Didar database. |
| HTTP 200 but body has `success: false` (or body is not valid JSON / cannot be deserialized) | Service returns a `HesabfaException` whose value is the Persian message `حسابفا کد خطا: {errorCode} -> {errorMessage}`. Via an implicit conversion it becomes the failure wrapper `HesabfaApiResult<T>` with `isSuccess=false`. | HTTP 200 with the failure wrapper JSON (see §9). A `RequestLog` is written to the Didar database. |
| HTTP 200 with `success: true` | Service returns the `result` payload. | HTTP 200 with the success wrapper JSON (see §9). |

Consequences for a provider implementation:

1. **Return HTTP 200 for both success and business errors.** Non-200 responses are treated as transport failures and surface to the client as an empty 200 response.
2. To report a business error, return `200` with `success: false` and a meaningful `errorCode` + `errorMessage`; the service embeds them in the client-facing message.
3. `errorCode`/`errorMessage` are surfaced only through the client-facing message text; no structured error fields are preserved.

---

## 8. Validation rules

- None of the request DTOs carry data annotations (`[Required]`, `[Range]`, etc.).
- The API host sets `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true`, so non-nullable reference-type properties (e.g. `apiKey`, `loginToken`) are **not** implicitly required.
- The only automatic validation that can reject a request before it reaches the service is ASP.NET Core model binding: empty body, malformed JSON, or type mismatches produce `HTTP 400` (framework `ValidationProblemDetails`).
- Within the forwarded body, the provider is free to apply its own validation and report problems through `success=false` (§7).

---

## 9. Client-facing contract (context)

The sections above fully describe the provider API. The following documents the other side of the same controller pipeline for completeness — useful if you are implementing a gateway that mimics the Accounting Service itself.

### 9.1 Client → Accounting Service

- Route template (from the base controller): `api/v{version:apiVersion}/[controller]/[action]`, API version `1`, so:
  `POST /api/v1/Hesabfa/{ActionName}` (e.g. `/api/v1/Hesabfa/GetInvoice`). Route matching is case-insensitive. Confirmed from the `Didar.Common` assembly metadata.
- HTTP method: `POST`. Content type: `application/json`.
- Controller is `[AllowAnonymous]` — no authentication required.
- Required header: `BaseUrl` (the provider base URL; read by the services).
- Request body: the action's request DTO (camelCase or any case — ASP.NET Core model binding is case-insensitive). Model binding is inferred as `[FromBody]` because the parameter is a complex type.
- No query string or route parameters for any action.

### 9.2 Response envelope the client sees

Every action returns `HesabfaApiResult<TData>` (`Hesabfa/Dtos/HesabfaApiResult.cs`), which inherits from `ApiResult` (external package `Didar.Common.WebFramework`). Its own properties are:

```json
{
  "isSuccess": true,
  "statusCode": <int>,
  "message": null,
  "data": { ... },          // the provider's `result`, re-serialized camelCase
  "errorMessage": null
}
```

- `data` / `errorMessage` are defined on `HesabfaApiResult<TData>`.
- `isSuccess`, `statusCode` (enum `ApiResultStatusCode`, serialized as a number), `message` and an `httpStatusCode` are inherited from `ApiResult`. The base class is defined in the external `Didar.Common` package **v1.0.6**; its exact serialized field set is confirmed only partially from binary inspection. Numeric values of the `ApiResultStatusCode` enum members are **`Unknown`** (external package).
- Success path: `{ isSuccess: true, statusCode: Success, message: null, data: <result>, errorMessage: null }`.
- Business-failure path: `{ isSuccess: false, statusCode: BadRequest, message: "حسابفا کد خطا: {errorCode} -> {errorMessage}", data: null, errorMessage: same message }`.
- HTTP status is **200** in both cases (the wrapper object is returned from the action, not a status-code result).
- Transport-failure path: HTTP 200 with an **empty body** (action returned `null`).

---

## 10. Logging side-effects

On any failure (transport error or `success=false`), the service writes a `RequestLog` record to the Didar database through `IExceptionLogService.Add`, capturing: `providerType`, `statusCode`, `httpMethod`, `requestBody` (JSON), `url`, `responseContent`, `exceptionMessage`, `userId`, `bizDomainId`. This is an internal side effect and does not change the HTTP response contract.

---

## 11. `Unknown` / not confirmed from code

- Numeric values of the `ApiResultStatusCode` enum (external `Didar.Common` package) used in the client-facing `statusCode` field.
- The full serialized property set of the base `ApiResult` type (partially confirmed from binary inspection only).
- Effective outbound timeout (code sets `RestClientOptions.MaxTimeout = -1`; semantics depend on RestSharp 112.x).
- Whether `priceList` on `ItemDto` has any specific shape (declared as `object?`).
- Whether `registrationDate` on contacts is a date, a string, or null (declared as `string?` in the DTO).
- Exact `Accept` header sent by RestSharp (not set explicitly in code).
- The `HesabfaKey` meaning on contacts (field name only).
- Any constraints the real Hesabfa service applies to `number`, `type`, `code`, `take`, `skip`, or filter operators — no validation exists in this codebase.

---

## 12. Source references

- Controller: `src/DidarInvoiceApi/Controllers/v1/HesabfaController.cs`
- Services: `Hesabfa/Services/SettingService.cs`, `Hesabfa/Services/ContactService.cs`, `Hesabfa/Services/HesabfaInvoiceService.cs`
- HTTP client wrapper: `Hesabfa/Services/Request.cs` (`Didar.Accounting.Hesabfa.Services.Request`)
- Interfaces: `Hesabfa/Abstraction/Setting/ISettingService.cs`, `Hesabfa/Abstraction/Contact/IContactService.cs`, `Hesabfa/Abstraction/Invoice/IHesabfaInvoiceService.cs`, `Hesabfa/Abstraction/IRequest.cs`
- DTOs: `Hesabfa/Dtos/**` (`HesabfaRequestBaseDto`, `HesabfaQueryInfoRequestBaseDto`, `HesabfaResponseBaseDto`, `HesabfaPaginationResponseBaseDto`, `HesabfaApiResult`, and the request/response DTOs under `Setting/`, `Contact/`, `Invoice/`, `Item/`)
- Exceptions: `Hesabfa/Exceptions/HesabfaException.cs`
- Domain: `src/Domain/Common/BaseDidarIdentity.cs`, `src/Domain/Enums/ProviderTypeEnum.cs`, `src/Domain/Helpers/EnumHelper.cs`, `src/Domain/Entities/RequestLog.cs`
- Host: `src/DidarInvoiceApi/Program.cs`, `src/DidarInvoiceApi/Middlewares/CustomExceptionHandlerMiddlewareExtensions.cs`
- External base framework: `Didar.Common` v1.0.6 (`Didar.Common.WebFramework` namespace; `ApiResult`, `ApiResultStatusCode`, `BaseController`).
