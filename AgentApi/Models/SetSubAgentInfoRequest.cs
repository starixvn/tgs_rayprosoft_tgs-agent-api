using System;

namespace TGS.Apis.AgentApi.Models;

#nullable enable

public class SetSubAgentInfoRequest
{
    public string? RealName { get; set; }

    public Guid SubAgentId { get; set; }
}
