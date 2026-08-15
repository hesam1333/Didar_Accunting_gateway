using Gateway.Application.Commands;
using Gateway.Domain.Enums;

namespace Gateway.Application.Commands;

public abstract record Command
{
    public Guid CommandId { get; init; } = Guid.NewGuid();

    public abstract CommandType CommandType { get; }
}
