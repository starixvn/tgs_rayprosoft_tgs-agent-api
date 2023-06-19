using System;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Models.ReportAgentWinLoss;

public class ExportAgentWinLossRequest
{
    public string Username { get; init; }
    public AccountType MemberType { get; init; }
    public bool DownLine { get; init; }
    public DateTimeOffset From { get; init; }
    public DateTimeOffset To { get; init; }
}