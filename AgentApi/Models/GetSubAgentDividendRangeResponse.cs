using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models;

public record GetSubAgentDividendRangeResponse : TgsResponse
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}
