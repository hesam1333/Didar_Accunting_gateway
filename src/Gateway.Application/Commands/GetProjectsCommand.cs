using Gateway.Domain.Enums;
namespace Gateway.Application.Commands;

public sealed record GetProjectsCommand : Command
{
    public override CommandType CommandType => CommandType.GetProjects;
}
