using System;

namespace TGS.Services.Agent.ApplicationServices.Player;

public record CreatePlayerQuery(Guid BrandId, Guid AgentId, string Username, string Password, string IpAddress,
    string HostName);
