namespace Gateway.Application.Dtos.Accounting.Responses;

public record ContactResponse
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Company { get; init; } = string.Empty;

    public int ContactType { get; init; }

    public string? NationalCode { get; init; }

    public string? EconomicCode { get; init; }

    public string? RegistrationNumber { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? Address { get; init; }

    public string? Country { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? PostalCode { get; init; }

    public string? Phone { get; init; }

    public string? Fax { get; init; }

    public string? Mobile { get; init; }

    public string? ContactEmail { get; init; }

    public string? Website { get; init; }

    public string? RegistrationDate { get; init; }

    public string? Note { get; init; }

    public bool IsShareHolder { get; init; }

    public bool IsCustomer { get; init; }

    public bool IsVendor { get; init; }

    public decimal SharePercent { get; init; }

    public decimal Liability { get; init; }

    public decimal Credits { get; init; }

    public decimal ContactCredit { get; init; }

    public int HesabfaKey { get; init; }

    public int NodeId { get; init; }

    public string? NodeName { get; init; }

    public string? NodeFamily { get; init; }

    public string? Tag { get; init; }

    public bool Active { get; init; }

    public int TaxType { get; init; }
}
