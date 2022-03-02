namespace TGS.Apis.AgentApi.Models
{
    public record AgentContributionResponse(
        int TotalCount,
        AgentContribution[] Items,
        AgentContributionTotal Total);
}
