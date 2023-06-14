using System;

namespace TGS.Apis.AgentApi.Models;

#nullable enable

public class SetSubAgentContactRequest
{
    public string? Email { get; set; }

    public string? Phone { get; set; }

    public Guid SubAgentId { get; set; }
}
