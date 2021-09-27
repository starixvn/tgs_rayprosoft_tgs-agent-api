using System;
using TGS.Core.Models.Enums;

namespace TGS.Clients.Report.Client.Models.Agent
{
    public record ExportAgentContributionsRequest(Guid BrandId, Guid? AgentId, string Username, AgentType? Type,
        DateTimeOffset From, DateTimeOffset To, bool IncludeAgents);
}