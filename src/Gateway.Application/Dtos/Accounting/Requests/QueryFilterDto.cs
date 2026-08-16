namespace Gateway.Application.Dtos.Accounting.Requests;

public record QueryFilterDto
{
    public string Property { get; init; } = string.Empty;

    public string Operator { get; init; } = string.Empty;

    public string Value { get; init; } = string.Empty;
}
