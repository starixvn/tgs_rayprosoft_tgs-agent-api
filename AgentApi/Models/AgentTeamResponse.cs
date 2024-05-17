using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models;

public record AgentTeamResponse : TgsResponse
{
    public AgentTeamData Data { get; init; }
}