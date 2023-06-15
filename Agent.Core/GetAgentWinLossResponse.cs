using TGS.Core.Models.Messages;

namespace TGS.Services.Agent;

public class GetAgentWinLossResponse : MessageResponse
{
    public AgentWinLossModel[]? Items { get; init; }

    public AgentWinLossTotalModel? Total { get; init; }

    public int TotalCount { get; init; }
}