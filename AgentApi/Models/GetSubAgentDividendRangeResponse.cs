using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models;

public record GetSubAgentDividendRangeResponse : TgsResponse
{
    public SubAgentDividendRange Data { get; set; }
}
