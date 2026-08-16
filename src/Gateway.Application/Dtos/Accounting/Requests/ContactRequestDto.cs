namespace Gateway.Application.Dtos.Accounting.Requests;

public record ContactRequestDto
{
    public string Code { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Company { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public int ContactType { get; init; }

    public string NodeFamily { get; init; } = string.Empty;

    public string? NationalCode { get; init; }

    public string? EconomicCode { get; init; }

    public string? RegistrationNumber { get; init; }

    public string? Email { get; init; }

    public string? Address { get; init; }

    public string? Country { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string? PostalCode { get; init; }

    public string? Phone { get; init; }

    public string? Fax { get; init; }

    public string? Mobile { get; init; }
}
