namespace Gateway.Application.Dtos.Accounting.Responses;

public record ProjectResponse
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public bool Active { get; init; }
}
