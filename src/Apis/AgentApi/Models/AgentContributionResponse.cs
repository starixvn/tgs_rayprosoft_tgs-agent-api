using Tgs.Report.Client.Models;
using Tgs.Report.Client.Models.Agent;

namespace TGS.Apis.AgentApi.Models
{
    public class AgentContributionResponse
    {
        public PagedResponse<AgentContributionData>? Datas { get; init; }

        public AgentContributionTotal Total { get; init; }
    }
}
