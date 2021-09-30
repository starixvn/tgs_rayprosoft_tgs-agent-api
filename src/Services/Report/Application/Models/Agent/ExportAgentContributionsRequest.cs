using System;
using System.IO;
using MediatR;
using TGS.Core.Models.Enums;

namespace TGS.Services.Report.Application.Models.Agent
{
    public record ExportAgentContributionsRequest(Guid BrandId, Guid? AgentId, string Username, AgentType? Type,
        DateTimeOffset From, DateTimeOffset To, bool IncludeAgents) : IRequest<MemoryStream>;
}