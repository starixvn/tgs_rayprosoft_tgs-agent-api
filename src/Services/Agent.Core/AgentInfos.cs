using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.Agent;

public class AgentInfos
{
    public Guid AgentId { get; set; }

    public AgentConfigStatus AgentStatus { get; set; }

    public decimal Balance { get; set; }

    public decimal CardGameRebatePercentage { get; set; }

    public decimal CommissionPercentage { get; set; }

    /// <summary>
    /// 佔成
    /// </summary>
    public decimal DividendPercentage { get; set; }

    public AgentLevelType LevelType { get; set; }

    public decimal LiveDealerRebatePercentage { get; set; }

    public decimal LotteryRebatePercentage { get; set; }

    public DateTimeOffset RegisteredOn { get; set; }

    public decimal SlotRebatePercentage { get; set; }

    public decimal SportsRebatePercentage { get; set; }

    public AgentStatus Status { get; set; }

    public int TeamNumber { get; set; }

    public AgentType Type { get; set; }

    public string Username { get; set; } = default!;

    public decimal VirtualSportsRebatePercentage { get; set; }
}
