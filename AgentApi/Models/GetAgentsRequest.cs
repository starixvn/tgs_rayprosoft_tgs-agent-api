using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Models;

public class GetAgentsRequest
{
    public AgentStatus? Status { get; set; }

    public string Username { get; set; }
}