using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent.ApplicationServices.AgentWinLossReport;

public class ExportAgentWinLossQuery
{
    public Guid BrandId { get; init; }
    public Guid AgentId { get; init; }
    public string Username { get; init; } = default!;
    public AccountType MemberType { get; init; }
    public bool DownLine { get; init; }
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
}